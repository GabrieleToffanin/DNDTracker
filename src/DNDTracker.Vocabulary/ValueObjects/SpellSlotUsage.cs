namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record SpellSlotUsage(
    int SlotLevel,
    int SlotsTotal,
    int SlotsSpent);
