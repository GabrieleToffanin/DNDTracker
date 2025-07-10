using DNDTracker.Domain.Campaigns;
using DNDTracker.Domain.Heroes.DomainEvents;
using DNDTracker.SharedKernel.Primitives;
using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.Exceptions;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Domain.Heroes;

public sealed class Hero : AggregateRoot<HeroId>
{
    public string Name { get; init; }
    public HeroClass Class { get; init; }
    public Race Race { get; init; }
    public Alignment Alignment { get; init; }
    public int Level { get; init; }
    public int Experience { get; init; }
    public int HitPoints { get; init; }
    public DiceType HitDice { get; init; }
    public HashSet<Spell> Spells { get; } = [];
    public Campaign Campaign { get; init; }
    
    private Hero(
        HeroId id,
        string name,
        HeroClass @class,
        Race race,
        Alignment alignment,
        int level,
        int experience,
        int hitPoints,
        DiceType hitDice) : base(id)
    { 
        this.Name = name;
        this.Class = @class;
        this.Race = race;
        this.Alignment = alignment;
        this.Level = level;
        this.Experience = experience;
        this.HitPoints = hitPoints;
        this.HitDice = hitDice;
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
        DiceType hitDice)
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
            hitDice);
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
        var currentId = HeroId.Create();
        
        return new Hero(
            currentId,
            name,
            @class,
            race,
            alignment,
            level,
            experience,
            hitPoints,
            hitDice);
    }

    /// <summary>
    /// Adds a spell to the hero's list of known spells if it is available for the hero to use.
    /// </summary>
    /// <param name="spell">The spell to be added to the hero's known spells.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided spell is null.</exception>
    /// <exception cref="SpellUnavailableException">Thrown when the spell is not available
    /// for the hero to use based on the hero's current level.</exception>
    public void AddSpell(Spell spell)
    {
        ArgumentNullException.ThrowIfNull(spell);
        
        if (!this.IsSpellAvailable(spell))
            SpellUnavailableException.Throw(
                message: "The spell is not available for the hero.");
        
        this.Spells.Add(spell);

        SpellLearnedDomainEvent spellLearnedDomainEvent = new(
            Guid.NewGuid(),
            DateTime.UtcNow,
            spell);
        
        this.AddDomainEvent(spellLearnedDomainEvent);
    }

    /// <summary>
    /// Determines whether a given spell is available for the hero to use
    /// based on the hero's current level.
    /// Logic has to be updated also on other params, but for simplicity we will leave it like this.
    /// </summary>
    /// <param name="spell">The spell to evaluate for availability.</param>
    /// <returns>True if the spell's level is less than or equal to the hero's level; otherwise, false.</returns>
    public bool IsSpellAvailable(Spell spell)
    {
        return spell.Level <= this.Level;
    }
}