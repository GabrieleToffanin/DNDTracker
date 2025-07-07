using DNDTracker.Application.Queries.UseCases.GetCampaign;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Outbounx.PostgresDb.Database.Postgres;
using DNDTracker.Outbounx.PostgresDb.Repositories;
using DNDTracker.Domain;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Inbound.RestAdapter.Controllers;
using DNDTracker.Outbound.InMemoryAdapter;
using DNDTracker.Main.Middleware;
using DNDTracker.Outbound.InMemoryAdapter.Messaging;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace DNDTracker.Main;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
            loggingBuilder.AddFilter("Microsoft.AspNetCore", LogLevel.Debug);
            loggingBuilder.AddFilter("Npgsql", LogLevel.Information);
            // Add OpenTelemetry logging
            loggingBuilder.AddFilter("OpenTelemetry", LogLevel.Debug);
            loggingBuilder.AddFilter("OpenTelemetry.Exporter.OpenTelemetryProtocol", LogLevel.Debug);
            loggingBuilder.AddFilter("OpenTelemetry.Exporter.Console", LogLevel.Debug);
        });

        builder.Services.AddDbContext<DNDTrackerPostgresDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]);
            options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
            options.EnableDetailedErrors(builder.Environment.IsDevelopment());
        });

        AssemblyPart inboundRestAdapterPart = 
            new AssemblyPart(typeof(CampaignController).Assembly);
        
        builder.Services.AddControllers()
            .PartManager.ApplicationParts.Add(inboundRestAdapterPart);
        
        builder.Services.AddOpenApi();
        builder.Services.AddMediatR(ConfigureMediatR);
        builder.Services.AddScoped<ICampaignRepository, PostgreCampaignRepository>();
        builder.Services.Configure<RabbitMqConfiguration>(
            builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.AddRabbitMqMessaging(builder.Configuration);
        
        builder.Services.Configure<BackpressureOptions>(
            builder.Configuration.GetSection("Backpressure"));
        builder.Services.AddSingleton<BackpressureOptions>(provider =>
            provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<BackpressureOptions>>().Value);
        
        // Configure OpenTelemetry
        ConfigureOpenTelemetry(builder);

        var app = builder.Build();
        
        ApplyMigrationsToPostgres(app);
        await app.Services.InitializeRabbitMqTopologyAsync();

        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options
                .WithTheme(ScalarTheme.Mars)
                .WithTitle("DNDTracker API")
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        // Only redirect to HTTPS in non-Docker environments
        if (!string.Equals(builder.Configuration["ASPNETCORE_ENVIRONMENT"], "Docker"))
        {
            app.UseHttpsRedirection();
        }
        
        app.UseMiddleware<BackpressureMiddleware>();
        app.UseAuthorization();
        app.MapControllers();

        // Add health check endpoint
        app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

        app.Run();
    }

    private static void ConfigureOpenTelemetry(WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("dndtracker-api", "1.0.0")
                        .AddAttributes(new Dictionary<string, object>
                        {
                            ["deployment.environment"] = builder.Environment.EnvironmentName,
                            ["service.instance.id"] = Environment.MachineName,
                            ["service.version"] = "1.0.0"
                        }))
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.Filter = httpContext =>
                        {
                            // Skip health check endpoints
                            return !httpContext.Request.Path.Value?.Contains("/health") == true;
                        };
                    })
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.SetDbStatementForStoredProcedure = true;
                        options.EnrichWithIDbCommand = (activity, command) =>
                        {
                            activity.SetTag("db.statement", command.CommandText);
                            activity.SetTag("db.connection_string", command.Connection?.ConnectionString);
                        };
                    })
                    .AddSource("DNDTracker.*")  // Custom application traces
                    .AddSource("RabbitMQ.*")    // RabbitMQ traces (preparazione per futuro uso)
                    .AddConsoleExporter()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://otel-collector:4317");
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                        options.TimeoutMilliseconds = 30000;
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("dndtracker-api", "1.0.0"))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMeter("DNDTracker.*")    // Custom application metrics
                    .AddMeter("RabbitMQ.*")      // RabbitMQ metrics (preparazione per futuro uso)
                    .AddConsoleExporter()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://otel-collector:4317");
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                        options.TimeoutMilliseconds = 30000;
                    });
            })
            .WithLogging(logging =>
            {
                logging
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("dndtracker-api", "1.0.0")
                        .AddAttributes(new Dictionary<string, object>
                        {
                            ["deployment.environment"] = builder.Environment.EnvironmentName,
                            ["service.instance.id"] = Environment.MachineName,
                            ["service.version"] = "1.0.0"
                        }))
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://otel-collector:4317");
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                        options.TimeoutMilliseconds = 30000;
                    });
            });
    }

    private static void ApplyMigrationsToPostgres(WebApplication app)
    {
        // Apply migrations during startup
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<DNDTrackerPostgresDbContext>();
                // Wait for PostgreSQL to be ready
                WaitForDbConnection(dbContext);
                // Apply migrations
                dbContext.Database.Migrate();
                app.Logger.LogInformation("Database migrations applied successfully");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating the database");
            }
        }
    }

    private static void WaitForDbConnection(DNDTrackerPostgresDbContext context, int retryCount = 5)
    {
        int currentRetry = 0;
        while (currentRetry < retryCount)
        {
            try
            {
                if (context.Database.CanConnect())
                    return;
            }
            catch (Exception ex)
            {
                currentRetry++;
                if (currentRetry >= retryCount)
                    throw;
            
                Console.WriteLine($"Database connection attempt {currentRetry} failed: {ex.Message}");
            }
        }
    }
    
    static void ConfigureMediatR(MediatRServiceConfiguration configuration)
    {
        configuration.RegisterServicesFromAssembly(typeof(GetCampaignByNameHandler).Assembly);
        configuration.RegisterServicesFromAssembly(typeof(CreateCampaignCommandHandler).Assembly);
    }
}