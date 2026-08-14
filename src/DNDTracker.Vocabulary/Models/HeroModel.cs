using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Vocabulary.Models;

public class HeroModel
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public HeroClass Class { get; set; }
    public Race Race { get; set; }
    public Alignment Alignment { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int HitPoints { get; set; }
    public DiceType HitDice { get; set; }
    public bool IsNonPlayerCharacter { get; set; }
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }
    public int CurrentHitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int TemporaryHitPoints { get; set; }
    public int ArmorClass { get; set; }
    public int Initiative { get; set; }
    public int Speed { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public List<InventoryItemModel> Inventory { get; private set; } = [];
    public List<EquipmentItemModel> Equipment { get; private set; } = [];
    public List<SpellbookEntryModel> Spellbook { get; private set; } = [];
    public List<SpellSlotUsageModel> SpellSlots { get; private set; } = [];
    public List<HeroConditionModel> Conditions { get; private set; } = [];
    public HashSet<SpellModel> Spells { get; } = [];
    public CampaignModel Campaign { get; set; } = null!;

    public void Apply(HeroModel source)
    {
        Name = source.Name;
        Class = source.Class;
        Race = source.Race;
        Alignment = source.Alignment;
        Level = source.Level;
        Experience = source.Experience;
        HitPoints = source.HitPoints;
        HitDice = source.HitDice;
        IsNonPlayerCharacter = source.IsNonPlayerCharacter;
        Strength = source.Strength;
        Dexterity = source.Dexterity;
        Constitution = source.Constitution;
        Intelligence = source.Intelligence;
        Wisdom = source.Wisdom;
        Charisma = source.Charisma;
        CurrentHitPoints = source.CurrentHitPoints;
        MaxHitPoints = source.MaxHitPoints;
        TemporaryHitPoints = source.TemporaryHitPoints;
        ArmorClass = source.ArmorClass;
        Initiative = source.Initiative;
        Speed = source.Speed;
        Notes = source.Notes;
        Background = source.Background;

        Synchronize(Inventory, source.Inventory, item => item.Id, (current, update) => current.Apply(update));
        Synchronize(Equipment, source.Equipment, item => item.Id, (current, update) => current.Apply(update));
        Synchronize(Spellbook, source.Spellbook, entry => entry.Id, (current, update) => current.Apply(update));
        Synchronize(SpellSlots, source.SpellSlots, slot => slot.Id, (current, update) => current.Apply(update));
        Synchronize(Conditions, source.Conditions, condition => condition.Id, (current, update) => current.Apply(update));
    }

    private static void Synchronize<TEntity, TKey>(
        List<TEntity> current,
        IEnumerable<TEntity> updates,
        Func<TEntity, TKey> keySelector,
        Action<TEntity, TEntity> apply)
        where TKey : notnull
    {
        var existingById = current.ToDictionary(keySelector);
        var updateKeys = new HashSet<TKey>();

        foreach (var update in updates)
        {
            var key = keySelector(update);
            updateKeys.Add(key);

            if (existingById.TryGetValue(key, out var existing))
                apply(existing, update);
            else
                current.Add(update);
        }

        current.RemoveAll(item => !updateKeys.Contains(keySelector(item)));
    }
}
