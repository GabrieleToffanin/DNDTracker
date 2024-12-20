using DNDTracker.Domain.Common;

namespace DNDTracker.Domain.DomainEvents;

public record HeroAddedDomainEvent(
    Guid Id,
    DateTime OccuredOn) 
    : DomainEvent(Id, OccuredOn);