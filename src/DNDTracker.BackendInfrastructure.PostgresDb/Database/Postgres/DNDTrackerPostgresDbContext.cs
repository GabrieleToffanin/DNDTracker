using Microsoft.EntityFrameworkCore;

namespace DNDTracker.BackendInfrastructure.PostgresDb.Database.Postgres;

public class DNDTrackerPostgresDbContext(
    DbContextOptions<DNDTrackerPostgresDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DNDTrackerPostgresDbContext).Assembly);
    }
}