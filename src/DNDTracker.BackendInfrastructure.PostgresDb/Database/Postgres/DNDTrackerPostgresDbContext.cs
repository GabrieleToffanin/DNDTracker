using DNDTracker.Domain;
using DNDTracker.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore;

namespace DNDTracker.BackendInfrastructure.PostgresDb.Database.Postgres;

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

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = new CancellationToken())
    {
        // This is a sort of outbox pattern. ( Easy )
        await PublishDomainEventsAsync(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = this.ChangeTracker
            .Entries<Entity>()
            .Where(entry => entry.Entity.DomainEvents is { Count: > 0 }).ToList();
            
        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
    }
}