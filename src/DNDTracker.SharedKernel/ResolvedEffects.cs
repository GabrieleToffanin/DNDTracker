using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.SharedKernel;

/// <summary>
/// Aggregated mechanical effects computed from a hero's active conditions and equipped items.
/// </summary>
public sealed record ResolvedEffects(
    int AttackBonus,
    int DamageBonus,
    IReadOnlyList<EffectToken> VariableAttackBonuses,
    IReadOnlyList<EffectToken> VariableDamageBonuses,
    bool HasAdvantageOnAttack,
    bool HasDisadvantageOnAttack,
    IReadOnlyList<string> Resistances,
    IReadOnlyList<string> Immunities,
    IReadOnlyList<string> Vulnerabilities,
    int SpeedModifier,
    IReadOnlyList<EffectToken> AllTokens)
{
    public static readonly ResolvedEffects Empty = new(
        AttackBonus: 0,
        DamageBonus: 0,
        VariableAttackBonuses: [],
        VariableDamageBonuses: [],
        HasAdvantageOnAttack: false,
        HasDisadvantageOnAttack: false,
        Resistances: [],
        Immunities: [],
        Vulnerabilities: [],
        SpeedModifier: 0,
        AllTokens: []);
}
