using DNDTracker.Domain.Common;
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
    
    public HashSet<Spell> Spells { get; init; } = [];
    
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
}