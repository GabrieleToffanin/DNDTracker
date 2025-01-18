using DNDTracker.Domain.Primitives;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Domain.Heroes.DomainEvents;

public record SpellLearnedDomainEvent(
    Guid Id,
    DateTime OccuredOn,
    Spell Spell) : DomainEvent(Id, OccuredOn);