namespace DNDTracker.Domain.ValueObjects;

public sealed record Spell
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Source { get; init; }
    public int Level { get; init; }
    public string? School { get; init; }
    public string? Time { get; init; }
    public string? Range { get; init; }
    public string? Components { get; init; }
    public string? Material { get; init; }
    public bool IsRitual { get; init; }
    public string? Duration { get; init; }
    public string? Concentration { get; init; }
    public string? CastingTime { get; init; }
    public string? Damage { get; init; }
    public string? Save { get; init; }
}