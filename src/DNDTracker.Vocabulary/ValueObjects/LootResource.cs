namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record LootResource(
    Guid Id,
    string Name,
    bool IsMagicItem,
    string Notes);
