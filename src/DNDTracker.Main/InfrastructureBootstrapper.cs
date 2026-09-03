using DNDTracker.Outbound.PostgresDb.Database.Postgres;
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
    }
}
