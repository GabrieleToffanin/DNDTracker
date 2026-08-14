namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record NpcResource(
    Guid Id,
    string Name,
    string Role,
    string Notes);
