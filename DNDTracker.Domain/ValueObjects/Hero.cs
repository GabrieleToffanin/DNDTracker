namespace DNDTracker.Domain.ValueObjects;

public sealed record Hero(
    string Name,
    string Class,
    int Level,
    IReadOnlyList<string> Abilities);