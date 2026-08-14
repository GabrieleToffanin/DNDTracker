using DNDTracker.Domain.Campaigns;
using DNDTracker.Domain.Heroes.DomainEvents;
using DNDTracker.SharedKernel.Primitives;
using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.Exceptions;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Domain.Heroes;

public sealed class Hero : AggregateRoot<HeroId>
{
    public string Name { get; private set; }
    public HeroClass Class { get; private set; }
    public Race Race { get; private set; }
    public Alignment Alignment { get; private set; }
    public int Level { get; private set; }
    public int Experience { get; private set; }
    public int HitPoints { get; private set; }
    public DiceType HitDice { get; private set; }
    public bool IsNonPlayerCharacter { get; private set; }
    public AbilityScores AbilityScores { get; private set; }
    public int CurrentHitPoints { get; private set; }
    public int MaxHitPoints { get; private set; }
    public int TemporaryHitPoints { get; private set; }
    public int ArmorClass { get; private set; }
    public int Initiative { get; private set; }
    public int Speed { get; private set; }
    public string Notes { get; private set; }
    public string Background { get; private set; }
    public List<InventoryItem> Inventory { get; private set; }
    public List<InventoryItem> Equipment { get; private set; }
    public List<CharacterSpellEntry> Spellbook { get; private set; }
    public List<SpellSlotUsage> SpellSlots { get; private set; }
    public List<CharacterCondition> Conditions { get; private set; }
    public HashSet<Spell> Spells { get; } = [];
    public Campaign? Campaign { get; private set; }

    private Hero(
        HeroId id,
        string name,
        HeroClass @class,
        Race race,
        Alignment alignment,
        int level,
        int experience,
        int hitPoints,
        DiceType hitDice,
        bool isNonPlayerCharacter,
        AbilityScores abilityScores,
        int currentHitPoints,
        int maxHitPoints,
        int temporaryHitPoints,
        int armorClass,
        int initiative,
        int speed,
        string notes,
        string background,
        IEnumerable<InventoryItem> inventory,
        IEnumerable<InventoryItem> equipment,
        IEnumerable<CharacterSpellEntry> spellbook,
        IEnumerable<SpellSlotUsage> spellSlots,
        IEnumerable<CharacterCondition> conditions) : base(id)
    {
        this.Name = name;
        this.Class = @class;
        this.Race = race;
        this.Alignment = alignment;
        this.Level = level;
        this.Experience = experience;
        this.HitPoints = hitPoints;
        this.HitDice = hitDice;
        this.IsNonPlayerCharacter = isNonPlayerCharacter;
        this.AbilityScores = abilityScores;
        this.CurrentHitPoints = currentHitPoints;
        this.MaxHitPoints = maxHitPoints;
        this.TemporaryHitPoints = temporaryHitPoints;
        this.ArmorClass = armorClass;
        this.Initiative = initiative;
        this.Speed = speed;
        this.Notes = notes;
        this.Background = background;
        this.Inventory = [.. inventory];
        this.Equipment = [.. equipment];
        this.Spellbook = [.. spellbook];
        this.SpellSlots = [.. spellSlots];
        this.Conditions = [.. conditions];
    }

    public static Hero Create(
        Guid? id,
        string name,
        HeroClass @class,
        Race race,
        Alignment alignment,
        int level,
        int experience,
        int hitPoints,
        DiceType hitDice,
        bool isNonPlayerCharacter,
        AbilityScores abilityScores,
        int currentHitPoints,
        int maxHitPoints,
        int temporaryHitPoints,
        int armorClass,
        int initiative,
        int speed,
        string notes,
        string background,
        IEnumerable<InventoryItem>? inventory = null,
        IEnumerable<InventoryItem>? equipment = null,
        IEnumerable<CharacterSpellEntry>? spellbook = null,
        IEnumerable<SpellSlotUsage>? spellSlots = null,
        IEnumerable<CharacterCondition>? conditions = null)
    {
        var currentId = id is not null ? HeroId.Create(id.Value) : HeroId.Create();

        return new Hero(
            currentId,
            name,
            @class,
            race,
            alignment,
            level,
            experience,
            hitPoints,
            hitDice,
            isNonPlayerCharacter,
            abilityScores,
            currentHitPoints,
            maxHitPoints,
            temporaryHitPoints,
            armorClass,
            initiative,
            speed,
            notes,
            background,
            inventory ?? [],
            equipment ?? [],
            spellbook ?? [],
            spellSlots ?? [],
            conditions ?? []);
    }

    public static Hero Create(
        string name,
        HeroClass @class,
        Race race,
        Alignment alignment,
        int level,
        int experience,
        int hitPoints,
        DiceType hitDice)
    {
        return Create(
            null,
            name,
            @class,
            race,
            alignment,
            level,
            experience,
            hitPoints,
            hitDice,
            false,
            AbilityScores.Default,
            hitPoints,
            hitPoints,
            0,
            10,
            0,
            30,
            string.Empty,
            string.Empty);
    }

    public void ApplyHitPointDelta(int damage, int healing, int temporaryHitPointsDelta)
    {
        if (damage < 0 || healing < 0)
            throw new ArgumentOutOfRangeException(nameof(damage), "Damage and healing must be non-negative.");

        this.TemporaryHitPoints = Math.Max(0, this.TemporaryHitPoints + temporaryHitPointsDelta);

        int remainingDamage = damage;
        if (remainingDamage > 0 && this.TemporaryHitPoints > 0)
        {
            int absorbed = Math.Min(this.TemporaryHitPoints, remainingDamage);
            this.TemporaryHitPoints -= absorbed;
            remainingDamage -= absorbed;
        }

        if (remainingDamage > 0)
            this.CurrentHitPoints = Math.Max(0, this.CurrentHitPoints - remainingDamage);

        if (healing > 0)
            this.CurrentHitPoints = Math.Min(this.MaxHitPoints, this.CurrentHitPoints + healing);
    }

    public void AddCondition(CharacterCondition condition)
    {
        ArgumentNullException.ThrowIfNull(condition);

        this.Conditions.RemoveAll(c => c.Name.Equals(condition.Name, StringComparison.OrdinalIgnoreCase));
        this.Conditions.Add(condition);
    }

    public void AddInventoryItem(InventoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        this.Inventory.Add(item);
    }

    public void AddEquipmentItem(InventoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        this.Equipment.Add(item);
    }

    public void SetSpellbook(IEnumerable<CharacterSpellEntry> spells, IEnumerable<SpellSlotUsage> spellSlots)
    {
        this.Spellbook = [.. spells];
        this.SpellSlots = [.. spellSlots];
    }

    public void UpdateNotes(string notes, string background)
    {
        this.Notes = notes;
        this.Background = background;
    }

    public void AddSpell(Spell spell)
    {
        ArgumentNullException.ThrowIfNull(spell);

        if (!this.IsSpellAvailable(spell))
            SpellUnavailableException.Throw("The spell is not available for the hero.");

        this.Spells.Add(spell);

        SpellLearnedDomainEvent spellLearnedDomainEvent = new(
            Guid.NewGuid(),
            DateTime.UtcNow,
            spell);

        this.AddDomainEvent(spellLearnedDomainEvent);
    }

    public bool IsSpellAvailable(Spell spell)
    {
        return spell.Level <= this.Level;
    }
}
