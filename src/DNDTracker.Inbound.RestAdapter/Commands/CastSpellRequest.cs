namespace DNDTracker.Inbound.RestAdapter.Commands;

public record CastSpellRequest(
    Guid TargetHeroId,
    int SpellId,
    int SlotLevel,
    int? DiceRoll = null);
