using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Vocabulary.ValueObjects;

/// <summary>
/// Represents a single parsed token from an effect code string.
/// Examples: ATK:2 → AttackBonus/2, DMG:1d6 fire → DamageBonus/1d6/fire,
/// SAVE:DEX:13 → SavingThrow/ability=Dex/DC=13, ADVATK → AdvantageAttack,
/// RESIST:slashing → Resistance/slashing.
/// </summary>
public sealed record EffectToken(
    EffectType EffectType,
    string? Magnitude = null,
    string? DamageType = null,
    string? AbilityOrSkill = null,
    int? DC = null);
