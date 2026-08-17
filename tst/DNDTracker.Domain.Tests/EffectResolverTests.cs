using DNDTracker.Domain.Heroes;
using DNDTracker.Domain.Services;
using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;
using FluentAssertions;

namespace DNDTracker.Domain.Tests;

public class EffectResolverTests
{
    private static Hero CreateHero(int level = 5) =>
        Hero.Create("Theron", HeroClass.Paladin, Race.Human, Alignment.Good, level, 0, 40, DiceType.D10);

    [Fact]
    public void ResolveEffects_NoConditions_ReturnsEmpty()
    {
        var hero = CreateHero();

        var result = EffectResolver.ResolveEffectsForCombatant(hero);

        result.AttackBonus.Should().Be(0);
        result.DamageBonus.Should().Be(0);
        result.HasAdvantageOnAttack.Should().BeFalse();
        result.Resistances.Should().BeEmpty();
    }

    [Fact]
    public void ResolveEffects_AttackBonusCondition_Aggregates()
    {
        var hero = CreateHero();
        hero.AddCondition(new CharacterCondition("Blessed", null, new EffectCode("ATK:2")));

        var result = EffectResolver.ResolveEffectsForCombatant(hero);

        result.AttackBonus.Should().Be(2);
    }

    [Fact]
    public void ResolveEffects_MultipleAttackBonuses_Stacks()
    {
        var hero = CreateHero();
        hero.AddCondition(new CharacterCondition("Bless", null, new EffectCode("ATK:2")));
        hero.AddCondition(new CharacterCondition("FightingStyle", null, new EffectCode("ATK:1")));

        var result = EffectResolver.ResolveEffectsForCombatant(hero);

        result.AttackBonus.Should().Be(3);
    }

    [Fact]
    public void ResolveEffects_AdvantageFlag_SetFromCondition()
    {
        var hero = CreateHero();
        hero.AddCondition(new CharacterCondition("Advantage", null, new EffectCode("ADVATK")));

        var result = EffectResolver.ResolveEffectsForCombatant(hero);

        result.HasAdvantageOnAttack.Should().BeTrue();
    }

    [Fact]
    public void ResolveEffects_Resistance_CollectedFromCondition()
    {
        var hero = CreateHero();
        hero.AddCondition(new CharacterCondition("Stone", null, new EffectCode("RESIST:slashing")));

        var result = EffectResolver.ResolveEffectsForCombatant(hero);

        result.Resistances.Should().Contain("slashing");
    }

    [Fact]
    public void ResolveEffects_EquippedItem_EffectCodeApplied()
    {
        var hero = CreateHero();
        hero.AddEquipmentItem(new InventoryItem(Guid.NewGuid(), "Ring of Protection", 1,
            EffectCode: new EffectCode("ATK:1")));

        var result = EffectResolver.ResolveEffectsForCombatant(hero);

        result.AttackBonus.Should().Be(1);
    }

    [Fact]
    public void ApplySpellEffect_HealToken_IncreasesTargetHP()
    {
        var caster = CreateHero();
        var target = CreateHero();
        target.ApplyHitPointDelta(20, 0, 0); // reduce HP to 20
        var spell = new Spell { Id = 1, Name = "Cure Wounds", Level = 1, EffectCode = new EffectCode("HEAL:8") };

        EffectResolver.ApplySpellEffect(caster, target, spell, diceRoll: 8);

        target.CurrentHitPoints.Should().Be(28);
    }

    [Fact]
    public void ApplySpellEffect_DamageToken_DecreasesTargetHP()
    {
        var caster = CreateHero();
        var target = CreateHero();
        var spell = new Spell { Id = 2, Name = "Firebolt", Level = 1, EffectCode = new EffectCode("DMG:8 fire") };

        EffectResolver.ApplySpellEffect(caster, target, spell, diceRoll: 8);

        target.CurrentHitPoints.Should().Be(32); // 40 - 8
    }

    [Fact]
    public void ApplySpellEffect_DamageToken_HalvedByResistance()
    {
        var caster = CreateHero();
        var target = CreateHero();
        target.AddCondition(new CharacterCondition("FireResist", null, new EffectCode("RESIST:fire")));
        var spell = new Spell { Id = 3, Name = "Fireball", Level = 3, EffectCode = new EffectCode("DMG:8 fire") };

        EffectResolver.ApplySpellEffect(caster, target, spell, diceRoll: 8);

        target.CurrentHitPoints.Should().Be(36); // 40 - 4 (halved)
    }

    [Fact]
    public void ApplySpellEffect_DamageToken_ZeroByImmunity()
    {
        var caster = CreateHero();
        var target = CreateHero();
        target.AddCondition(new CharacterCondition("FireImmune", null, new EffectCode("IMMUNE:fire")));
        var spell = new Spell { Id = 4, Name = "Fireball", Level = 3, EffectCode = new EffectCode("DMG:10 fire") };

        EffectResolver.ApplySpellEffect(caster, target, spell, diceRoll: 10);

        target.CurrentHitPoints.Should().Be(40); // no damage
    }

    [Fact]
    public void ApplySpellEffect_RaisesSpellCastDomainEvent_OnCaster()
    {
        var caster = CreateHero();
        var target = CreateHero();
        var spell = new Spell { Id = 5, Name = "Magic Missile", Level = 1, EffectCode = new EffectCode("DMG:3") };

        EffectResolver.ApplySpellEffect(caster, target, spell, diceRoll: 3);

        caster.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void ApplySpellEffect_ConditionToken_AddsConditionToTarget()
    {
        var caster = CreateHero();
        var target = CreateHero();
        var spell = new Spell { Id = 6, Name = "Hold Person", Level = 2, EffectCode = new EffectCode("COND:Paralyzed") };

        EffectResolver.ApplySpellEffect(caster, target, spell);

        target.Conditions.Should().Contain(c => c.Name == "Paralyzed");
    }

    [Fact]
    public void TickHeroConditions_DecrementsRemainingRounds()
    {
        var hero = CreateHero();
        hero.AddCondition(new CharacterCondition("Haste", 3, new EffectCode("SPEED:30")));

        EffectResolver.TickHeroConditions(hero);

        hero.Conditions.Should().Contain(c => c.Name == "Haste" && c.RemainingRounds == 2);
    }

    [Fact]
    public void TickHeroConditions_RemovesExpiredConditions()
    {
        var hero = CreateHero();
        hero.AddCondition(new CharacterCondition("Poison", 1, new EffectCode("DMG:2")));

        EffectResolver.TickHeroConditions(hero);

        hero.Conditions.Should().NotContain(c => c.Name == "Poison");
    }

    [Fact]
    public void TickHeroConditions_HealToken_HealsHeroEachTurn()
    {
        var hero = CreateHero();
        hero.ApplyHitPointDelta(10, 0, 0); // reduce to 30
        hero.AddCondition(new CharacterCondition("Regeneration", null, new EffectCode("HEAL:5")));

        EffectResolver.TickHeroConditions(hero);

        hero.CurrentHitPoints.Should().Be(35);
    }

    [Fact]
    public void Hero_ProficiencyBonus_CorrectForLevel1()
    {
        var hero = CreateHero(level: 1);
        hero.ProficiencyBonus.Should().Be(2);
    }

    [Fact]
    public void Hero_ProficiencyBonus_CorrectForLevel5()
    {
        var hero = CreateHero(level: 5);
        hero.ProficiencyBonus.Should().Be(3);
    }

    [Fact]
    public void Hero_UseSpellSlot_DecreasesAvailableSlots()
    {
        var hero = Hero.Create(null, "Gandalf", HeroClass.Wizard, Race.Human, Alignment.Good, 5, 0, 30, DiceType.D6,
            false, AbilityScores.Default, 30, 30, 0, 12, 0, 30, "", "",
            spellSlots: [new SpellSlotUsage(1, 4, 0)]);

        hero.UseSpellSlot(1);

        hero.SpellSlots[0].SlotsSpent.Should().Be(1);
    }

    [Fact]
    public void Hero_UseSpellSlot_ThrowsWhenNoSlotsAvailable()
    {
        var hero = Hero.Create(null, "Gandalf", HeroClass.Wizard, Race.Human, Alignment.Good, 5, 0, 30, DiceType.D6,
            false, AbilityScores.Default, 30, 30, 0, 12, 0, 30, "", "",
            spellSlots: [new SpellSlotUsage(1, 2, 2)]);

        var act = () => hero.UseSpellSlot(1);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Hero_RecoverSpellSlots_LongRest_RestoresAll()
    {
        var hero = Hero.Create(null, "Gandalf", HeroClass.Wizard, Race.Human, Alignment.Good, 5, 0, 30, DiceType.D6,
            false, AbilityScores.Default, 30, 30, 0, 12, 0, 30, "", "",
            spellSlots: [new SpellSlotUsage(1, 4, 3), new SpellSlotUsage(2, 3, 2)]);

        hero.RecoverSpellSlots(RestType.Long);

        hero.SpellSlots.Should().AllSatisfy(s => s.SlotsSpent.Should().Be(0));
    }

    [Fact]
    public void Hero_RecoverSpellSlots_ShortRest_Warlock_RestoresAll()
    {
        var hero = Hero.Create(null, "Warwick", HeroClass.Warlock, Race.Human, Alignment.Chaotic, 5, 0, 30, DiceType.D8,
            false, AbilityScores.Default, 30, 30, 0, 12, 0, 30, "", "",
            spellSlots: [new SpellSlotUsage(3, 2, 2)]);

        hero.RecoverSpellSlots(RestType.Short);

        hero.SpellSlots[0].SlotsSpent.Should().Be(0);
    }

    [Fact]
    public void Hero_RecoverSpellSlots_ShortRest_NonWarlock_NoRecovery()
    {
        var hero = Hero.Create(null, "Aria", HeroClass.Bard, Race.Human, Alignment.Neutral, 5, 0, 30, DiceType.D8,
            false, AbilityScores.Default, 30, 30, 0, 12, 0, 30, "", "",
            spellSlots: [new SpellSlotUsage(1, 4, 3)]);

        hero.RecoverSpellSlots(RestType.Short);

        hero.SpellSlots[0].SlotsSpent.Should().Be(3);
    }
}
