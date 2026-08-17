namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record InventoryItem(
    Guid Id,
    string Name,
    int Quantity,
    string? Notes = null,
    EffectCode? EffectCode = null);
