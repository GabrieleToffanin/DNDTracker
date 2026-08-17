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
    string Alignment,
    string Statistics,
    string Actions,
    string? Notes = null,
    string Description = "",
    string BonusActions = "",
    string Reactions = "",
    string LegendaryActions = "",
    string LairActions = "",
    string Spells = "");
