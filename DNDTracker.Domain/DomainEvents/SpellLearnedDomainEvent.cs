using DNDTracker.Domain.Common;
using DNDTracker.Domain.ValueObjects;

namespace DNDTracker.Domain.DomainEvents;

public record SpellLearnedDomainEvent(
    Guid Id,
    DateTime OccuredOn,
    Spell Spell) : DomainEvent(Id, OccuredOn);