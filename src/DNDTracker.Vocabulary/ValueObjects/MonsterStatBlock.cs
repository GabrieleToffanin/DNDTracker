namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record MonsterStatBlock(
    Guid Id,
    string Name,
    string CreatureType,
    int ArmorClass,
    int HitPoints,
    int ChallengeRating,
    int ExperiencePoints,
    int InitiativeModifier,
    int Speed,
    string? Notes = null,
    string Description = "",
    string Statistics = "",
    string Actions = "",
    string BonusActions = "",
    string Reactions = "",
    string LegendaryActions = "",
    string LairActions = "",
    string Spells = "");
