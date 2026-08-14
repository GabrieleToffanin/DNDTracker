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
    public string? Notes { get; set; }
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
        Notes = monster.Notes
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
        Notes);

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
        Notes = source.Notes;
    }
}
