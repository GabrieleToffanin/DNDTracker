using DNDTracker.Domain.Primitives;

namespace DNDTracker.Domain.Campaigns.DomainEvents;

public record HeroAddedDomainEvent(
    Guid Id,
    DateTime OccuredOn) 
    : DomainEvent(Id, OccuredOn);
    