using DNDTracker.Domain.Common;
using DNDTracker.Domain.DomainEvents;
using DNDTracker.Domain.Exceptions;
using DNDTracker.Domain.ValueObjects;

namespace DNDTracker.Domain.Entities;

public sealed class Hero : AggregateRoot<HeroId>
{
    public HeroId Id { get; init; }
    public string Name { get; init; }
    public string Class { get; init; }
    public string Race { get; init; }
    public string Alignment { get; init; }
    public int Level { get; init; }
    public int Experience { get; init; }
    public int HitPoints { get; init; }
    public int HitDice { get; init; }
    public HashSet<Spell> Spells { get; } = [];
    public Campaign Campaign { get; init; }
    
    private Hero(
        HeroId id,
        string name,
        string @class,
        string race,
        string alignment,
        int level,
        int experience,
        int hitPoints,
        int hitDice) : base(id)
    { 
        this.Id = id;
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
        HeroId id,
        string name,
        string @class,
        string race,
        string alignment,
        int level,
        int experience,
        int hitPoints,
        int hitDice)
    {
        return new Hero(
            id,
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
    private bool IsSpellAvailable(Spell spell)
    {
        return spell.Level <= this.Level;
    }
}