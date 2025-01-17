using Microsoft.EntityFrameworkCore;

namespace DNDTracker.BackendInfrastructure.PostgresDb.Database.Postgres;

public class DNDTrackerPostgresDbContext : DbContext
{
    public DNDTrackerPostgresDbContext(DbContextOptions<DNDTrackerPostgresDbContext> options) 
        : base(options)
    {
    }

    public DNDTrackerPostgresDbContext(string connectionString)
        : base(GetOptions(connectionString))
    {
        
    }
    
    private static DbContextOptions GetOptions(string connectionString)
    {
        return new DbContextOptionsBuilder<DNDTrackerPostgresDbContext>()
            .UseNpgsql(connectionString)
            .Options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DNDTrackerPostgresDbContext).Assembly);
    }
}