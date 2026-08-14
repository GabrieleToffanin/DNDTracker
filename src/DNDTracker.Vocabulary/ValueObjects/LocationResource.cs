namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record LocationResource(
    Guid Id,
    string Name,
    string Description,
    string? MapUrl);
