using System.Runtime.CompilerServices;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Domain.Campaigns;

public sealed class MonsterStatBlock
{
    private MonsterStatBlock(
        Guid id,
        string name,
        string creatureType,
        int armorClass,
        int hitPoints,
        int challengeRating,
        int experiencePoints,
        int initiativeModifier,
        int speed,
        string alignment,
        string statistics,
        string actions,
        string? notes,
        string description,
        string bonusActions,
        string reactions,
        string legendaryActions,
        string lairActions,
        string spells)
    {
        Id = id;
        Name = name;
        CreatureType = creatureType;
        ArmorClass = armorClass;
        HitPoints = hitPoints;
        ChallengeRating = challengeRating;
        ExperiencePoints = experiencePoints;
        InitiativeModifier = initiativeModifier;
        Speed = speed;
        Alignment = alignment;
        Statistics = statistics;
        Actions = actions;
        Notes = notes;
        Description = description;
        BonusActions = bonusActions;
        Reactions = reactions;
        LegendaryActions = legendaryActions;
        LairActions = lairActions;
        Spells = spells;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string CreatureType { get; private set; }
    public int ArmorClass { get; private set; }
    public int HitPoints { get; private set; }
    public int ChallengeRating { get; private set; }
    public int ExperiencePoints { get; private set; }
    public int InitiativeModifier { get; private set; }
    public int Speed { get; private set; }
    public string Alignment { get; private set; }
    public string Statistics { get; private set; }
    public string Actions { get; private set; }
    public string? Notes { get; private set; }
    public string Description { get; private set; }
    public string BonusActions { get; private set; }
    public string Reactions { get; private set; }
    public string LegendaryActions { get; private set; }
    public string LairActions { get; private set; }
    public string Spells { get; private set; }

    public static MonsterStatBlock Create(
        Guid? id,
        string name,
        string creatureType,
        int armorClass,
        int hitPoints,
        int challengeRating,
        int experiencePoints,
        int initiativeModifier,
        int speed,
        string alignment,
        string statistics,
        string actions,
        string? notes = null,
        string description = "",
        string bonusActions = "",
        string reactions = "",
        string legendaryActions = "",
        string lairActions = "",
        string spells = "")
    {
        ThrowIfInvalidName(name);
        ThrowIfInvalidCreatureType(creatureType);
        ThrowIfNegative(armorClass, nameof(armorClass));
        ThrowIfNegative(hitPoints, nameof(hitPoints));
        ThrowIfNegative(challengeRating, nameof(challengeRating));
        ThrowIfNegative(experiencePoints, nameof(experiencePoints));
        ThrowIfNegative(speed, nameof(speed));

        return new MonsterStatBlock(
            id is { } value && value != Guid.Empty ? value : Guid.NewGuid(),
            name,
            creatureType,
            armorClass,
            hitPoints,
            challengeRating,
            experiencePoints,
            initiativeModifier,
            speed,
            alignment,
            statistics,
            actions,
            notes,
            description,
            bonusActions,
            reactions,
            legendaryActions,
            lairActions,
            spells);
    }

    public void EnsureIdentity()
    {
        if (Id == Guid.Empty)
            Id = Guid.NewGuid();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIfInvalidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidCampaignDataException("Invalid monster name.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIfInvalidCreatureType(string creatureType)
    {
        if (string.IsNullOrWhiteSpace(creatureType))
            throw new InvalidCampaignDataException("Invalid monster creature type.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIfNegative(int value, string parameterName)
    {
        if (value < 0)
            throw new InvalidCampaignDataException($"Invalid monster {parameterName}.");
    }
}
