using Microsoft.EntityFrameworkCore;

namespace DNDTracker.Infrastructure.Database.Postgres;

public class DNDTrackerPostgresDbContext : DbContext
{
    public DNDTrackerPostgresDbContext(DbContextOptions<DNDTrackerPostgresDbContext> options)
        : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DNDTrackerPostgresDbContext).Assembly);
    }
}