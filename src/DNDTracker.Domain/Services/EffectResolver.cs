using DNDTracker.Domain.Heroes;
using DNDTracker.SharedKernel;
using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Domain.Services;

/// <summary>
/// Domain service that resolves and applies effect codes for heroes.
/// </summary>
public static class EffectResolver
{
    /// <summary>
    /// Aggregates all active effect codes from a hero's conditions and equipped items
    /// into a single <see cref="ResolvedEffects"/> snapshot.
    /// </summary>
    public static ResolvedEffects ResolveEffectsForCombatant(Hero hero)
    {
        ArgumentNullException.ThrowIfNull(hero);

        var allTokens = new List<EffectToken>();

        foreach (var condition in hero.Conditions)
        {
            if (condition.EffectCode is not null)
                allTokens.AddRange(condition.EffectCode.ParsedTokens);
        }

        foreach (var item in hero.Equipment)
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

    /// <summary>
    /// Applies the effect code tokens of a spell to the target hero, handling HP changes
    /// and condition application, and raises a <see cref="SpellCastDomainEvent"/> on the caster.
    /// </summary>
    public static void ApplySpellEffect(Hero caster, Hero target, Spell spell, int? diceRoll = null)
    {
        ArgumentNullException.ThrowIfNull(caster);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(spell);

        if (spell.EffectCode is null)
        {
            RaiseSpellCastEvent(caster, target, spell);
            return;
        }

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
                    int dmg = ResolveNumericValue(token.Magnitude, diceRoll);
                    int finalDmg = ApplyDamageModifiers(target, dmg, token.DamageType);
                    target.ApplyHitPointDelta(finalDmg, 0, 0);
                    break;
                }
                case EffectType.TemporaryHitPoints:
                {
                    int tmpHp = ResolveNumericValue(token.Magnitude, diceRoll);
                    target.ApplyHitPointDelta(0, 0, tmpHp);
                    break;
                }
                case EffectType.Condition when token.AbilityOrSkill is not null:
                {
                    target.AddCondition(new CharacterCondition(token.AbilityOrSkill, null));
                    break;
                }
            }
        }

        RaiseSpellCastEvent(caster, target, spell);
    }

    private static void RaiseSpellCastEvent(Hero caster, Hero target, Spell spell)
    {
        caster.RecordSpellCast(target.Id.Id, spell);
    }

    /// <summary>
    /// Applies per-turn condition ticks to a hero: decrements RemainingRounds and
    /// automatically applies Heal/DamageBonus effect code tokens.
    /// </summary>
    public static void TickHeroConditions(Hero hero)
    {
        ArgumentNullException.ThrowIfNull(hero);

        foreach (var condition in hero.Conditions.Where(c => c.EffectCode is not null))
        {
            foreach (var token in condition.EffectCode!.ParsedTokens)
            {
                switch (token.EffectType)
                {
                    case EffectType.Heal:
                    {
                        int healAmount = ResolveNumericValue(token.Magnitude, null);
                        if (healAmount > 0)
                            hero.ApplyHitPointDelta(0, healAmount, 0);
                        break;
                    }
                    case EffectType.DamageBonus:
                    {
                        int dmg = ResolveNumericValue(token.Magnitude, null);
                        if (dmg > 0)
                        {
                            int finalDmg = ApplyDamageModifiers(hero, dmg, token.DamageType);
                            hero.ApplyHitPointDelta(finalDmg, 0, 0);
                        }
                        break;
                    }
                }
            }
        }

        hero.TickConditions();
    }

    private static int ApplyDamageModifiers(Hero target, int rawDamage, string? damageType)
    {
        var resolved = ResolveEffectsForCombatant(target);

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

        // If it is a plain integer return it
        if (int.TryParse(magnitude, out var intValue))
            return intValue;

        // If a dice roll was provided use it directly (the caller handles rolling)
        return diceRoll ?? 0;
    }

    private static bool TryParseInt(string? value, out int result)
    {
        result = 0;
        return value is not null && int.TryParse(value, out result);
    }
}
