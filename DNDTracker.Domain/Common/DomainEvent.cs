namespace DNDTracker.Domain.Common;

public record DomainEvent(Guid Id, DateTime OccuredOn, string EventType);