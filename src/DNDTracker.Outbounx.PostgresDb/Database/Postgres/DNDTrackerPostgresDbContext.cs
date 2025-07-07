using DNDTracker.Domain;
using DNDTracker.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore;

namespace DNDTracker.Outbounx.PostgresDb.Database.Postgres;

public class DNDTrackerPostgresDbContext : DbContext
{
    private readonly IEventPublisher _eventPublisher;
    
    public DNDTrackerPostgresDbContext(
        DbContextOptions<DNDTrackerPostgresDbContext> options,
        IEventPublisher eventPublisher) 
        : base(options)
    {
        _eventPublisher = eventPublisher;
    }

    public DNDTrackerPostgresDbContext(
        string connectionString)
        : base(GetOptions(connectionString))
    {
        
    }
    
    private static DbContextOptions GetOptions(
        string connectionString)
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

// Campaign
// -- Evento ho aggiunto un eroe

// Save
// -- Prendi tutti gli eventi di dominio dentro le Entity
// -- Salvo gli eventi in una lista temp
// -- Pulisci gli eventi di dominio dentro le Entity
