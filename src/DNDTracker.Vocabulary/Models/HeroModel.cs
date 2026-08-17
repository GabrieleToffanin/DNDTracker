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

    // Richness fields — scalars
    public int DeathSaveSuccesses { get; set; }
    public int DeathSaveFailures { get; set; }
    public string PersonalityTraits { get; set; } = string.Empty;
    public string Ideals { get; set; } = string.Empty;
    public string Bonds { get; set; } = string.Empty;
    public string Flaws { get; set; } = string.Empty;
    public AbilityType? SpellcastingAbility { get; set; }

    // Richness fields — relational collections
    public List<HeroSavingThrowProficiencyModel> SavingThrowProficiencies { get; private set; } = [];
    public List<HeroSkillProficiencyModel> SkillProficiencies { get; private set; } = [];
    public List<HeroFeatModel> Feats { get; private set; } = [];

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
        DeathSaveSuccesses = source.DeathSaveSuccesses;
        DeathSaveFailures = source.DeathSaveFailures;
        PersonalityTraits = source.PersonalityTraits;
        Ideals = source.Ideals;
        Bonds = source.Bonds;
        Flaws = source.Flaws;
        SpellcastingAbility = source.SpellcastingAbility;

        SynchronizeSimple(SavingThrowProficiencies, source.SavingThrowProficiencies, m => m.Id);
        SynchronizeSimple(SkillProficiencies, source.SkillProficiencies, m => m.Id);
        SynchronizeSimple(Feats, source.Feats, m => m.Id);

        Synchronize(Inventory, source.Inventory, item => item.Id, (current, update) => current.Apply(update));
        Synchronize(Equipment, source.Equipment, item => item.Id, (current, update) => current.Apply(update));
        var desiredSpellIds = source.Spells.Select(spell => spell.Id).ToHashSet();
        Spells.RemoveWhere(spell => !desiredSpellIds.Contains(spell.Id));
        var existingSpellIds = Spells.Select(spell => spell.Id).ToHashSet();
        foreach (var spell in source.Spells.Where(spell => !existingSpellIds.Contains(spell.Id)))
            Spells.Add(spell);
        Synchronize(Spellbook, source.Spellbook, entry => entry.Id, (current, update) => current.Apply(update));
        Synchronize(SpellSlots, source.SpellSlots, slot => slot.Id, (current, update) => current.Apply(update));
        Synchronize(Conditions, source.Conditions, condition => condition.Id, (current, update) => current.Apply(update));
    }

    // For simple models that are replaced wholesale (no field-level apply needed)
    private static void SynchronizeSimple<TEntity, TKey>(
        List<TEntity> current,
        IEnumerable<TEntity> updates,
        Func<TEntity, TKey> keySelector)
        where TKey : notnull
    {
        var updateList = updates.ToList();
        var updateKeys = updateList.Select(keySelector).ToHashSet();
        current.RemoveAll(item => !updateKeys.Contains(keySelector(item)));
        var existingKeys = current.Select(keySelector).ToHashSet();
        current.AddRange(updateList.Where(u => !existingKeys.Contains(keySelector(u))));
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
