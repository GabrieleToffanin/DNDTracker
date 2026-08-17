using DNDTracker.SharedKernel.Primitives;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Domain.Heroes.DomainEvents;

public record SpellCastDomainEvent(
    Guid Id,
    DateTime OccuredOn,
    Guid CasterHeroId,
    Guid TargetHeroId,
    Spell Spell,
    int SlotLevel) : DomainEvent(Id, OccuredOn);
