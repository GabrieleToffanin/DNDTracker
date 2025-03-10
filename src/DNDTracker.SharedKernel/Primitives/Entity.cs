namespace DNDTracker.SharedKernel.Primitives;

public abstract class Entity
{
    private readonly List<DomainEvent> _domainEvents = [];
    
    public List<DomainEvent> DomainEvents => _domainEvents;
    
    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    protected void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}