using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class MonsterStatBlockModel
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatureType { get; set; } = string.Empty;
    public int ArmorClass { get; set; }
    public int HitPoints { get; set; }
    public int ChallengeRating { get; set; }
    public int ExperiencePoints { get; set; }
    public int InitiativeModifier { get; set; }
    public int Speed { get; set; }
    public string Alignment { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Statistics { get; set; } = string.Empty;
    public string Actions { get; set; } = string.Empty;
    public string BonusActions { get; set; } = string.Empty;
    public string Reactions { get; set; } = string.Empty;
    public string LegendaryActions { get; set; } = string.Empty;
    public string LairActions { get; set; } = string.Empty;
    public string Spells { get; set; } = string.Empty;
    public CampaignModel Campaign { get; set; } = null!;

    public static MonsterStatBlockModel From(MonsterStatBlock monster) => new()
    {
        Id = monster.Id,
        Name = monster.Name,
        CreatureType = monster.CreatureType,
        ArmorClass = monster.ArmorClass,
        HitPoints = monster.HitPoints,
        ChallengeRating = monster.ChallengeRating,
        ExperiencePoints = monster.ExperiencePoints,
        InitiativeModifier = monster.InitiativeModifier,
        Speed = monster.Speed,
        Alignment = monster.Alignment,
        Notes = monster.Notes,
        Description = monster.Description,
        Statistics = monster.Statistics,
        Actions = monster.Actions,
        BonusActions = monster.BonusActions,
        Reactions = monster.Reactions,
        LegendaryActions = monster.LegendaryActions,
        LairActions = monster.LairActions,
        Spells = monster.Spells
    };

    public MonsterStatBlock ToValueObject() => new(
        Id,
        Name,
        CreatureType,
        ArmorClass,
        HitPoints,
        ChallengeRating,
        ExperiencePoints,
        InitiativeModifier,
        Speed,
        Alignment,
        Statistics,
        Actions,
        Notes,
        Description,
        BonusActions,
        Reactions,
        LegendaryActions,
        LairActions,
        Spells);

    public void Apply(MonsterStatBlockModel source)
    {
        Name = source.Name;
        CreatureType = source.CreatureType;
        ArmorClass = source.ArmorClass;
        HitPoints = source.HitPoints;
        ChallengeRating = source.ChallengeRating;
        ExperiencePoints = source.ExperiencePoints;
        InitiativeModifier = source.InitiativeModifier;
        Speed = source.Speed;
        Alignment = source.Alignment;
        Notes = source.Notes;
        Description = source.Description;
        Statistics = source.Statistics;
        Actions = source.Actions;
        BonusActions = source.BonusActions;
        Reactions = source.Reactions;
        LegendaryActions = source.LegendaryActions;
        LairActions = source.LairActions;
        Spells = source.Spells;
    }
}
