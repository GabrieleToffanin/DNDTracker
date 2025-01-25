namespace DNDTracker.SharedKernel.Primitives;

public record DomainEvent(Guid Id, DateTime OccuredOn);