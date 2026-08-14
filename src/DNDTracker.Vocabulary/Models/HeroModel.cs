using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Vocabulary.Models;

public class HeroModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public HeroClass Class { get; init; }
    public Race Race { get; init; }
    public Alignment Alignment { get; init; }
    public int Level { get; init; }
    public int Experience { get; init; }
    public int HitPoints { get; init; }
    public DiceType HitDice { get; init; }
    public bool IsNonPlayerCharacter { get; init; }
    public int Strength { get; init; }
    public int Dexterity { get; init; }
    public int Constitution { get; init; }
    public int Intelligence { get; init; }
    public int Wisdom { get; init; }
    public int Charisma { get; init; }
    public int CurrentHitPoints { get; init; }
    public int MaxHitPoints { get; init; }
    public int TemporaryHitPoints { get; init; }
    public int ArmorClass { get; init; }
    public int Initiative { get; init; }
    public int Speed { get; init; }
    public string Notes { get; init; } = string.Empty;
    public string Background { get; init; } = string.Empty;
    public string InventoryJson { get; init; } = "[]";
    public string EquipmentJson { get; init; } = "[]";
    public string SpellbookJson { get; init; } = "[]";
    public string SpellSlotsJson { get; init; } = "[]";
    public string ConditionsJson { get; init; } = "[]";
    public HashSet<SpellModel> Spells { get; } = [];
    public CampaignModel Campaign { get; init; }
}
