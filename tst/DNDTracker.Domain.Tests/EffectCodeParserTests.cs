using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;
using FluentAssertions;

namespace DNDTracker.Domain.Tests;

public class EffectCodeParserTests
{
    [Fact]
    public void Parse_AttackBonus_Token()
    {
        var code = new EffectCode("ATK:2");

        code.ParsedTokens.Should().HaveCount(1);
        code.ParsedTokens[0].EffectType.Should().Be(EffectType.AttackBonus);
        code.ParsedTokens[0].Magnitude.Should().Be("2");
    }

    [Fact]
    public void Parse_DamageBonus_Token_With_SpaceSeparatedDamageType()
    {
        var code = new EffectCode("DMG:1d6 fire");

        code.ParsedTokens.Should().HaveCount(1);
        code.ParsedTokens[0].EffectType.Should().Be(EffectType.DamageBonus);
        code.ParsedTokens[0].Magnitude.Should().Be("1d6");
        code.ParsedTokens[0].DamageType.Should().Be("fire");
    }

    [Fact]
    public void Parse_DamageBonus_Token_With_ColonSeparatedDamageType()
    {
        var code = new EffectCode("DMG:1d6:slashing");

        code.ParsedTokens.Should().HaveCount(1);
        code.ParsedTokens[0].EffectType.Should().Be(EffectType.DamageBonus);
        code.ParsedTokens[0].Magnitude.Should().Be("1d6");
        code.ParsedTokens[0].DamageType.Should().Be("slashing");
    }

    [Fact]
    public void Parse_SavingThrow_Token()
    {
        var code = new EffectCode("SAVE:DEX:13");

        code.ParsedTokens.Should().HaveCount(1);
        code.ParsedTokens[0].EffectType.Should().Be(EffectType.SavingThrow);
        code.ParsedTokens[0].AbilityOrSkill.Should().Be("DEX");
        code.ParsedTokens[0].DC.Should().Be(13);
    }

    [Fact]
    public void Parse_AdvantageAttack_Token()
    {
        var code = new EffectCode("ADVATK");

        code.ParsedTokens.Should().HaveCount(1);
        code.ParsedTokens[0].EffectType.Should().Be(EffectType.AdvantageAttack);
    }

    [Fact]
    public void Parse_Resistance_Token()
    {
        var code = new EffectCode("RESIST:slashing");

        code.ParsedTokens.Should().HaveCount(1);
        code.ParsedTokens[0].EffectType.Should().Be(EffectType.Resistance);
        code.ParsedTokens[0].DamageType.Should().Be("slashing");
    }

    [Fact]
    public void Parse_Immunity_Token()
    {
        var code = new EffectCode("IMMUNE:fire");

        code.ParsedTokens[0].EffectType.Should().Be(EffectType.Immunity);
        code.ParsedTokens[0].DamageType.Should().Be("fire");
    }

    [Fact]
    public void Parse_Heal_Token()
    {
        var code = new EffectCode("HEAL:5");

        code.ParsedTokens[0].EffectType.Should().Be(EffectType.Heal);
        code.ParsedTokens[0].Magnitude.Should().Be("5");
    }

    [Fact]
    public void Parse_TemporaryHitPoints_Token()
    {
        var code = new EffectCode("TEMPHP:10");

        code.ParsedTokens[0].EffectType.Should().Be(EffectType.TemporaryHitPoints);
        code.ParsedTokens[0].Magnitude.Should().Be("10");
    }

    [Fact]
    public void Parse_Condition_Token()
    {
        var code = new EffectCode("COND:Stunned");

        code.ParsedTokens[0].EffectType.Should().Be(EffectType.Condition);
        code.ParsedTokens[0].AbilityOrSkill.Should().Be("Stunned");
    }

    [Fact]
    public void Parse_Speed_Token()
    {
        var code = new EffectCode("SPEED:-10");

        code.ParsedTokens[0].EffectType.Should().Be(EffectType.Speed);
        code.ParsedTokens[0].Magnitude.Should().Be("-10");
    }

    [Fact]
    public void Parse_MultiToken_BlessedEffect()
    {
        var code = new EffectCode("ATK:1d4; SAVE:1d4");

        code.ParsedTokens.Should().HaveCount(2);
        code.ParsedTokens[0].EffectType.Should().Be(EffectType.AttackBonus);
        code.ParsedTokens[1].EffectType.Should().Be(EffectType.SavingThrow);
    }

    [Fact]
    public void Parse_FullComplexCode()
    {
        var code = new EffectCode("ATK:2; DMG:1d6 fire; SAVE:DEX:13; ADVATK; RESIST:slashing");

        code.ParsedTokens.Should().HaveCount(5);
        code.ParsedTokens[0].EffectType.Should().Be(EffectType.AttackBonus);
        code.ParsedTokens[1].EffectType.Should().Be(EffectType.DamageBonus);
        code.ParsedTokens[2].EffectType.Should().Be(EffectType.SavingThrow);
        code.ParsedTokens[3].EffectType.Should().Be(EffectType.AdvantageAttack);
        code.ParsedTokens[4].EffectType.Should().Be(EffectType.Resistance);
    }

    [Fact]
    public void Parse_UnknownTokens_AreIgnored()
    {
        var code = new EffectCode("UNKNOWN:foo; ATK:1");

        code.ParsedTokens.Should().HaveCount(1);
        code.ParsedTokens[0].EffectType.Should().Be(EffectType.AttackBonus);
    }

    [Fact]
    public void TryCreate_ReturnsNull_ForNullOrWhiteSpace()
    {
        EffectCode.TryCreate(null).Should().BeNull();
        EffectCode.TryCreate("   ").Should().BeNull();
    }

    [Fact]
    public void TryCreate_ReturnsParsedCode_ForValidInput()
    {
        var code = EffectCode.TryCreate("ATK:2");
        code.Should().NotBeNull();
        code!.ParsedTokens.Should().HaveCount(1);
    }

    [Fact]
    public void Raw_RoundTrips_ToString()
    {
        const string raw = "ATK:2; RESIST:fire";
        var code = new EffectCode(raw);
        code.ToString().Should().Be(raw);
    }
}
