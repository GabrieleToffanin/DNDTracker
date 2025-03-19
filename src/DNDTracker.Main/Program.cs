using DNDTracker.Application.Queries.UseCases.GetCampaign;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Outbounx.PostgresDb.Database.Postgres;
using DNDTracker.Outbounx.PostgresDb.Repositories;
using DNDTracker.Domain;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Inbound.RestAdapter.Controllers;
using DNDTracker.Outbound.InMemoryAdapter.Messaging;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace DNDTracker.Main;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
            loggingBuilder.AddFilter("Microsoft.AspNetCore", LogLevel.Debug);
        });

        builder.Services.AddDbContext<DNDTrackerPostgresDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]);
        });

        AssemblyPart inboundRestAdapterPart = 
            new AssemblyPart(typeof(CampaignController).Assembly);
        
        builder.Services.AddControllers()
            .PartManager.ApplicationParts.Add(inboundRestAdapterPart);
        builder.Services.AddOpenApi();
        builder.Services.AddMediatR(ConfigureMediatR);
        builder.Services.AddScoped<ICampaignRepository, PostgreCampaignRepository>();
        builder.Services.AddSingleton<IEventPublisher, EventPublisher>();
        
        var app = builder.Build();
        
        ApplyMigrationsToPostgres(app);

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
        
        app.MapControllers();

        app.Run();
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