using System.Diagnostics;
using DNDTracker.Application.Queries.UseCases.GetCampaign;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Domain;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Inbound.AmqpAdapter;
using DNDTracker.Inbound.RestAdapter.Controllers;
using DNDTracker.Main.Middleware;
using DNDTracker.Outbound.RabbitMq;
using DNDTracker.Outbound.RabbitMq.Configuration;
using DNDTracker.Outbounx.PostgresDb.Database.Postgres;
using DNDTracker.Outbounx.PostgresDb.Repositories;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scalar.AspNetCore;
using Serilog;

namespace DNDTracker.Main;

public class Program
{
    public static async Task Main(string[] args)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Npgsql", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console();
        });

        builder.Services.AddDbContext<DNDTrackerPostgresDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]);
            options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
            options.EnableDetailedErrors(builder.Environment.IsDevelopment());
        });

        AssemblyPart inboundRestAdapterPart = new(typeof(CampaignController).Assembly);

        builder.Services.AddControllers()
            .PartManager.ApplicationParts.Add(inboundRestAdapterPart);

        builder.Services.AddOpenApi();
        builder.Services.AddMediatR(ConfigureMediatR);
        builder.Services.AddScoped<ICampaignRepository, PostgreCampaignRepository>();
        builder.Services.Configure<RabbitMqConfiguration>(
            builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.AddRabbitMqMessaging();
        builder.Services.AddAmqpAdapter();

        builder.Services.Configure<BackpressureOptions>(
            builder.Configuration.GetSection("Backpressure"));
        builder.Services.AddSingleton<BackpressureOptions>(provider =>
            provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<BackpressureOptions>>().Value);

        WebApplication app = builder.Build();

        ApplyMigrationsToPostgres(app);
        await app.Services.InitializeRabbitMqTopologyAsync();

        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("DNDTracker API")
                .WithTheme(ScalarTheme.Mars)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        if (!string.Equals(builder.Configuration["ASPNETCORE_ENVIRONMENT"], "Docker"))
        {
            app.UseHttpsRedirection();
        }

        app.UseMiddleware<DatadogLogCorrelationMiddleware>();
        app.UseMiddleware<BackpressureMiddleware>();
        app.UseAuthorization();
        app.MapControllers();
        app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

        app.Run();
    }

    private static void ApplyMigrationsToPostgres(WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        try
        {
            DNDTrackerPostgresDbContext dbContext = services.GetRequiredService<DNDTrackerPostgresDbContext>();
            WaitForDbConnection(dbContext);
            dbContext.Database.Migrate();
            app.Logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database");
        }
    }

    private static void WaitForDbConnection(DNDTrackerPostgresDbContext context, int retryCount = 30, int delayMs = 1000)
    {
        int currentRetry = 0;
        while (currentRetry < retryCount)
        {
            try
            {
                if (context.Database.CanConnect())
                {
                    Console.WriteLine("✅ Database connection established");
                    return;
                }
            }
            catch (Exception ex)
            {
                currentRetry++;
                if (currentRetry >= retryCount)
                {
                    throw;
                }

                Console.WriteLine($"⏳ Database connection attempt {currentRetry}/{retryCount} failed: {ex.Message}");
                Thread.Sleep(delayMs);
            }
        }
    }

    private static void ConfigureMediatR(MediatRServiceConfiguration configuration)
    {
        configuration.RegisterServicesFromAssembly(typeof(GetCampaignByNameHandler).Assembly);
        configuration.RegisterServicesFromAssembly(typeof(CreateCampaignCommandHandler).Assembly);
    }
}
