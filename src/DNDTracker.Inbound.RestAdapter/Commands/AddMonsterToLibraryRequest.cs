namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record AddMonsterToLibraryRequest(
    string Name,
    string CreatureType,
    int ArmorClass,
    int HitPoints,
    int ChallengeRating,
    int ExperiencePoints,
    int InitiativeModifier,
    int Speed,
    string? Notes);
