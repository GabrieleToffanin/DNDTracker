using DNDTracker.Domain.Heroes;
using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Xunit;

namespace DNDTracker.Application.Tests;

public class SpellAvailabilityPropertyTests
{
    [Property]
    public void Spells_Are_Available_For_Valid_Hero_Levels(PositiveInt heroLevel)
    {
        // Use positive integers for hero level
        var spell = new Spell { Level = heroLevel.Get - 1 };
        var hero = CreateHero(heroLevel.Get);

        // Property: Any spell with level less than hero's level should be available
        Assert.True(hero.IsSpellAvailable(spell));
    }

    [Property]
    public void Spells_Are_Unavailable_When_Level_Is_Insufficient(PositiveInt spellLevel, PositiveInt heroLevel)
    {
        // Only test when spell level exceeds hero level
        Prop.When(spellLevel.Get > heroLevel.Get, () =>
        {
            var spell = new Spell { Level = spellLevel.Get };
            var hero = CreateHero(heroLevel.Get);

            // Property: Any spell with level more than hero's level should be unavailable
            Assert.False(hero.IsSpellAvailable(spell));
        });
    }

    [Property]
    public void Same_Level_Spells_Are_Available_For_Hero(PositiveInt level)
    {
        var spell = new Spell { Level = level.Get };
        var hero = CreateHero(level.Get);

        // Property: A spell with the same level as the hero should be available
        Assert.True(hero.IsSpellAvailable(spell));
    }

    // Helper method to create a Hero with specific level
    private static Hero CreateHero(int level) => Hero.Create(
        name: "Test Hero",
        @class: HeroClass.Barbarian,
        race: Race.Human,
        alignment: Alignment.Chaotic,
        level: level,
        experience: 0,
        hitPoints: 10,
        hitDice: DiceType.D10
    );
}