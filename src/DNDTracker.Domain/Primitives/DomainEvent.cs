namespace DNDTracker.Domain.Primitives;

public record DomainEvent(Guid Id, DateTime OccuredOn);