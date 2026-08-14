using DNDTracker.Outbound.PostgresDb.Database.Postgres;
using DNDTracker.Outbound.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace DNDTracker.Main;

internal static class InfrastructureBootstrapper
{
    public static async Task InitializeAsync(WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        app.Logger.LogInformation("Initializing PostgreSQL migrations...");
        DNDTrackerPostgresDbContext dbContext = services.GetRequiredService<DNDTrackerPostgresDbContext>();
        await dbContext.Database.MigrateAsync();
        app.Logger.LogInformation("PostgreSQL migrations completed.");

        app.Logger.LogInformation("Initializing RabbitMQ topology...");
        await app.Services.InitializeRabbitMqTopologyAsync();
        app.Logger.LogInformation("Database migrations and RabbitMQ topology initialized successfully");
    }
}
