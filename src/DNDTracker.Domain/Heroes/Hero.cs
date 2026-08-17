using DNDTracker.Domain.Campaigns;
using DNDTracker.Domain.Heroes.DomainEvents;
using DNDTracker.SharedKernel;
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

    // Richness fields
    public DeathSaves DeathSaves { get; private set; }
    public SavingThrowProficiencies SavingThrowProficiencies { get; private set; }
    public SkillProficiencies SkillProficiencies { get; private set; }
    public CharacterPersonality Personality { get; private set; }
    public List<string> Feats { get; private set; }
    public AbilityType? SpellcastingAbility { get; private set; }

    /// <summary>Proficiency bonus derived from character level (D&amp;D 5e standard).</summary>
    public int ProficiencyBonus => Level switch
    {
        >= 17 => 6,
        >= 13 => 5,
        >= 9  => 4,
        >= 5  => 3,
        _     => 2
    };

    /// <summary>Passive Perception = 10 + Wisdom modifier + proficiency bonus (if proficient).</summary>
    public int PassivePerception
    {
        get
        {
            var wisdomMod = (AbilityScores.Wisdom - 10) / 2;
            var profBonus = SkillProficiencies.IsProficient(SkillType.Perception) ? ProficiencyBonus : 0;
            return 10 + wisdomMod + profBonus;
        }
    }

    /// <summary>Spell save DC = 8 + proficiency bonus + spellcasting ability modifier.</summary>
    public int SpellSaveDC
    {
        get
        {
            if (SpellcastingAbility is null)
                return 0;
            var mod = GetAbilityModifier(SpellcastingAbility.Value);
            return 8 + ProficiencyBonus + mod;
        }
    }

    /// <summary>Spell attack bonus = proficiency bonus + spellcasting ability modifier.</summary>
    public int SpellAttackBonus
    {
        get
        {
            if (SpellcastingAbility is null)
                return 0;
            return ProficiencyBonus + GetAbilityModifier(SpellcastingAbility.Value);
        }
    }

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
        DeathSaves deathSaves,
        SavingThrowProficiencies savingThrowProficiencies,
        SkillProficiencies skillProficiencies,
        CharacterPersonality personality,
        IEnumerable<string> feats,
        AbilityType? spellcastingAbility,
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
        this.DeathSaves = deathSaves;
        this.SavingThrowProficiencies = savingThrowProficiencies;
        this.SkillProficiencies = skillProficiencies;
        this.Personality = personality;
        this.Feats = [.. feats];
        this.SpellcastingAbility = spellcastingAbility;
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
        IEnumerable<CharacterCondition>? conditions = null,
        DeathSaves? deathSaves = null,
        SavingThrowProficiencies? savingThrowProficiencies = null,
        SkillProficiencies? skillProficiencies = null,
        CharacterPersonality? personality = null,
        IEnumerable<string>? feats = null,
        AbilityType? spellcastingAbility = null)
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
            deathSaves ?? DeathSaves.None,
            savingThrowProficiencies ?? SavingThrowProficiencies.None,
            skillProficiencies ?? SkillProficiencies.None,
            personality ?? CharacterPersonality.Empty,
            feats ?? [],
            spellcastingAbility,
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

    public void TickConditions()
    {
        this.Conditions = this.Conditions
            .Select(c => c.RemainingRounds is > 0 ? c with { RemainingRounds = c.RemainingRounds - 1 } : c)
            .Where(c => c.RemainingRounds is null || c.RemainingRounds > 0)
            .ToList();
    }

    public ResolvedEffects ResolveEffects()
    {
        var allTokens = new List<EffectToken>();

        foreach (var condition in this.Conditions)
        {
            if (condition.EffectCode is not null)
                allTokens.AddRange(condition.EffectCode.ParsedTokens);
        }

        foreach (var item in this.Equipment)
        {
            if (item.EffectCode is not null)
                allTokens.AddRange(item.EffectCode.ParsedTokens);
        }

        int attackBonus = 0;
        int damageBonus = 0;
        bool advantageAttack = false;
        bool disadvantageAttack = false;
        int speedModifier = 0;
        var resistances = new List<string>();
        var immunities = new List<string>();
        var vulnerabilities = new List<string>();

        foreach (var token in allTokens)
        {
            switch (token.EffectType)
            {
                case EffectType.AttackBonus when TryParseInt(token.Magnitude, out var v):
                    attackBonus += v;
                    break;
                case EffectType.DamageBonus when TryParseInt(token.Magnitude, out var v):
                    damageBonus += v;
                    break;
                case EffectType.AdvantageAttack:
                    advantageAttack = true;
                    break;
                case EffectType.DisadvantageAttack:
                    disadvantageAttack = true;
                    break;
                case EffectType.Resistance when token.DamageType is not null:
                    resistances.Add(token.DamageType);
                    break;
                case EffectType.Immunity when token.DamageType is not null:
                    immunities.Add(token.DamageType);
                    break;
                case EffectType.Vulnerability when token.DamageType is not null:
                    vulnerabilities.Add(token.DamageType);
                    break;
                case EffectType.Speed when TryParseInt(token.Magnitude, out var v):
                    speedModifier += v;
                    break;
            }
        }

        return new ResolvedEffects(
            AttackBonus: attackBonus,
            DamageBonus: damageBonus,
            HasAdvantageOnAttack: advantageAttack,
            HasDisadvantageOnAttack: disadvantageAttack,
            Resistances: resistances,
            Immunities: immunities,
            Vulnerabilities: vulnerabilities,
            SpeedModifier: speedModifier,
            AllTokens: allTokens);
    }

    public void ApplySpellEffectTo(Hero target, Spell spell, int? diceRoll = null)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(spell);

        if (spell.EffectCode is null)
        {
            this.RecordSpellCast(target.Id.Id, spell);
            return;
        }

        var targetEffects = target.ResolveEffects();

        foreach (var token in spell.EffectCode.ParsedTokens)
        {
            switch (token.EffectType)
            {
                case EffectType.Heal:
                {
                    int healAmount = ResolveNumericValue(token.Magnitude, diceRoll);
                    target.ApplyHitPointDelta(0, healAmount, 0);
                    break;
                }
                case EffectType.DamageBonus:
                {
                    int damage = ResolveNumericValue(token.Magnitude, diceRoll);
                    int finalDamage = target.ApplyDamageModifiers(damage, token.DamageType, targetEffects);
                    target.ApplyHitPointDelta(finalDamage, 0, 0);
                    break;
                }
                case EffectType.TemporaryHitPoints:
                {
                    int temporaryHitPoints = ResolveNumericValue(token.Magnitude, diceRoll);
                    target.ApplyHitPointDelta(0, 0, temporaryHitPoints);
                    break;
                }
                case EffectType.Condition when token.AbilityOrSkill is not null:
                {
                    target.AddCondition(new CharacterCondition(token.AbilityOrSkill, null));
                    break;
                }
            }
        }

        this.RecordSpellCast(target.Id.Id, spell);
    }

    public void TickOngoingEffects()
    {
        var resolvedEffects = this.ResolveEffects();

        foreach (var condition in this.Conditions.Where(c => c.EffectCode is not null))
        {
            foreach (var token in condition.EffectCode!.ParsedTokens)
            {
                switch (token.EffectType)
                {
                    case EffectType.Heal:
                    {
                        int healAmount = ResolveNumericValue(token.Magnitude, null);
                        if (healAmount > 0)
                            this.ApplyHitPointDelta(0, healAmount, 0);
                        break;
                    }
                    case EffectType.DamageBonus:
                    {
                        int damage = ResolveNumericValue(token.Magnitude, null);
                        if (damage > 0)
                        {
                            int finalDamage = this.ApplyDamageModifiers(damage, token.DamageType, resolvedEffects);
                            this.ApplyHitPointDelta(finalDamage, 0, 0);
                        }
                        break;
                    }
                }
            }
        }

        this.TickConditions();
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

    public void UpdatePersonality(CharacterPersonality personality)
    {
        ArgumentNullException.ThrowIfNull(personality);
        this.Personality = personality;
    }

    public void AddFeat(string featName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(featName);
        if (!this.Feats.Contains(featName, StringComparer.OrdinalIgnoreCase))
            this.Feats.Add(featName);
    }

    public void SetSpellcastingAbility(AbilityType? ability)
    {
        this.SpellcastingAbility = ability;
    }

    public void RecordDeathSave(bool success)
    {
        if (success)
            this.DeathSaves = this.DeathSaves with { Successes = Math.Min(3, this.DeathSaves.Successes + 1) };
        else
            this.DeathSaves = this.DeathSaves with { Failures = Math.Min(3, this.DeathSaves.Failures + 1) };
    }

    public void ResetDeathSaves()
    {
        this.DeathSaves = DeathSaves.None;
    }

    public void UseSpellSlot(int slotLevel)
    {
        var slot = this.SpellSlots.FirstOrDefault(s => s.SlotLevel == slotLevel);
        if (slot is null || slot.SlotsSpent >= slot.SlotsTotal)
            throw new InvalidOperationException($"No available spell slots at level {slotLevel}.");

        var index = this.SpellSlots.IndexOf(slot);
        this.SpellSlots[index] = slot with { SlotsSpent = slot.SlotsSpent + 1 };
    }

    public void RecoverSpellSlots(RestType restType)
    {
        bool isWarlock = this.Class == HeroClass.Warlock;
        bool isFighter = this.Class == HeroClass.Fighter;

        if (restType == RestType.Long)
        {
            // Full recovery on long rest for all classes
            this.SpellSlots = this.SpellSlots
                .Select(s => s with { SlotsSpent = 0 })
                .ToList();
            return;
        }

        // Short rest: Warlock recovers all pact magic slots; Fighter (Arcane Knight) recovers one low-level slot
        if (isWarlock)
        {
            this.SpellSlots = this.SpellSlots
                .Select(s => s with { SlotsSpent = 0 })
                .ToList();
        }
        else if (isFighter)
        {
            // Arcane Knight recovers one spell slot up to level 3 on short rest
            var recoverableSlot = this.SpellSlots
                .Where(s => s.SlotLevel <= 3 && s.SlotsSpent > 0)
                .OrderBy(s => s.SlotLevel)
                .FirstOrDefault();

            if (recoverableSlot is not null)
            {
                var index = this.SpellSlots.IndexOf(recoverableSlot);
                this.SpellSlots[index] = recoverableSlot with { SlotsSpent = recoverableSlot.SlotsSpent - 1 };
            }
        }
        // Other classes do not recover spell slots on short rest
    }

    public void RecordSpellCast(Guid targetHeroId, Spell spell, int? slotLevel = null)
    {
        this.AddDomainEvent(new SpellCastDomainEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            this.Id.Id,
            targetHeroId,
            spell,
            slotLevel ?? spell.Level));
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

    private int ApplyDamageModifiers(int rawDamage, string? damageType, ResolvedEffects? resolvedEffects = null)
    {
        var resolved = resolvedEffects ?? this.ResolveEffects();

        if (damageType is not null)
        {
            if (resolved.Immunities.Any(i => i.Equals(damageType, StringComparison.OrdinalIgnoreCase)))
                return 0;
            if (resolved.Resistances.Any(r => r.Equals(damageType, StringComparison.OrdinalIgnoreCase)))
                return rawDamage / 2;
            if (resolved.Vulnerabilities.Any(v => v.Equals(damageType, StringComparison.OrdinalIgnoreCase)))
                return rawDamage * 2;
        }

        return rawDamage;
    }

    private static int ResolveNumericValue(string? magnitude, int? diceRoll)
    {
        if (magnitude is null)
            return diceRoll ?? 0;

        if (int.TryParse(magnitude, out var intValue))
            return intValue;

        return diceRoll ?? 0;
    }

    private static bool TryParseInt(string? value, out int result)
    {
        result = 0;
        return value is not null && int.TryParse(value, out result);
    }

    public int GetAbilityModifier(AbilityType ability)
    {
        var score = ability switch
        {
            AbilityType.Strength     => AbilityScores.Strength,
            AbilityType.Dexterity    => AbilityScores.Dexterity,
            AbilityType.Constitution => AbilityScores.Constitution,
            AbilityType.Intelligence => AbilityScores.Intelligence,
            AbilityType.Wisdom       => AbilityScores.Wisdom,
            AbilityType.Charisma     => AbilityScores.Charisma,
            _ => 10
        };
        return (score - 10) / 2;
    }
}
