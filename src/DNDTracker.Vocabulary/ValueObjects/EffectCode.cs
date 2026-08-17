using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Vocabulary.ValueObjects;

/// <summary>
/// A machine-readable effect code string inspired by Fantasy Grounds / BetterCombatEffects.
/// Format: semicolon-separated tokens, e.g.
///   "ATK:2; DMG:1d6 fire; SAVE:DEX:13; ADVATK; RESIST:slashing"
///
/// Supported tokens:
///   ATK:&lt;magnitude&gt;              → AttackBonus
///   DMG:&lt;magnitude&gt; [damageType] → DamageBonus
///   SAVE:&lt;ability&gt;:&lt;DC&gt;          → SavingThrow
///   ADVATK                        → AdvantageAttack
///   DISADVATK                     → DisadvantageAttack
///   RESIST:&lt;damageType&gt;          → Resistance
///   IMMUNE:&lt;damageType&gt;          → Immunity
///   VULN:&lt;damageType&gt;            → Vulnerability
///   TEMPHP:&lt;magnitude&gt;           → TemporaryHitPoints
///   HEAL:&lt;magnitude&gt;             → Heal
///   COND:&lt;conditionName&gt;         → Condition
///   AURA:&lt;magnitude&gt;             → Aura
///   SPEED:&lt;magnitude&gt;            → Speed
///   ABILCHECK:&lt;ability&gt;:&lt;bonus&gt; → AbilityCheck
///   SKILLCHECK:&lt;skill&gt;:&lt;bonus&gt;  → SkillCheck
/// </summary>
public sealed record EffectCode
{
    public string Raw { get; }
    public IReadOnlyList<EffectToken> ParsedTokens { get; }

    public EffectCode(string raw)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(raw);
        Raw = raw;
        ParsedTokens = Parse(raw);
    }

    public static EffectCode? TryCreate(string? raw)
        => string.IsNullOrWhiteSpace(raw) ? null : new EffectCode(raw);

    private static IReadOnlyList<EffectToken> Parse(string raw)
    {
        var tokens = new List<EffectToken>();

        foreach (var segment in raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var token = ParseToken(segment);
            if (token is not null)
                tokens.Add(token);
        }

        return tokens;
    }

    private static EffectToken? ParseToken(string segment)
    {
        var parts = segment.Split(':', StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
            return null;

        var keyword = parts[0].ToUpperInvariant();

        return keyword switch
        {
            "ATK" => new EffectToken(EffectType.AttackBonus, Magnitude: parts.ElementAtOrDefault(1)),

            "DMG" => ParseDmgToken(parts),

            "SAVE" => ParseSaveToken(parts),

            "ADVATK" => new EffectToken(EffectType.AdvantageAttack),

            "DISADVATK" => new EffectToken(EffectType.DisadvantageAttack),

            "RESIST" => new EffectToken(EffectType.Resistance, DamageType: parts.ElementAtOrDefault(1)),

            "IMMUNE" => new EffectToken(EffectType.Immunity, DamageType: parts.ElementAtOrDefault(1)),

            "VULN" => new EffectToken(EffectType.Vulnerability, DamageType: parts.ElementAtOrDefault(1)),

            "TEMPHP" => new EffectToken(EffectType.TemporaryHitPoints, Magnitude: parts.ElementAtOrDefault(1)),

            "HEAL" => new EffectToken(EffectType.Heal, Magnitude: parts.ElementAtOrDefault(1)),

            "COND" => new EffectToken(EffectType.Condition, AbilityOrSkill: parts.ElementAtOrDefault(1)),

            "AURA" => new EffectToken(EffectType.Aura, Magnitude: parts.ElementAtOrDefault(1)),

            "SPEED" => new EffectToken(EffectType.Speed, Magnitude: parts.ElementAtOrDefault(1)),

            "ABILCHECK" => new EffectToken(
                EffectType.AbilityCheck,
                Magnitude: parts.ElementAtOrDefault(2),
                AbilityOrSkill: parts.ElementAtOrDefault(1)),

            "SKILLCHECK" => new EffectToken(
                EffectType.SkillCheck,
                Magnitude: parts.ElementAtOrDefault(2),
                AbilityOrSkill: parts.ElementAtOrDefault(1)),

            _ => null
        };
    }

    /// <summary>
    /// DMG token supports an optional damage type after a space in the magnitude segment:
    /// e.g. "DMG:1d6 fire" — parts[0]=DMG, parts[1]="1d6 fire"
    /// or   "DMG:1d6:fire" — parts[0]=DMG, parts[1]="1d6", parts[2]="fire"
    /// </summary>
    private static EffectToken ParseDmgToken(string[] parts)
    {
        var magnitudeRaw = parts.ElementAtOrDefault(1);
        string? magnitude = null;
        string? damageType = null;

        if (magnitudeRaw is not null)
        {
            var spaceIndex = magnitudeRaw.IndexOf(' ');
            if (spaceIndex >= 0)
            {
                magnitude = magnitudeRaw[..spaceIndex].Trim();
                damageType = magnitudeRaw[(spaceIndex + 1)..].Trim();
            }
            else
            {
                magnitude = magnitudeRaw;
                damageType = parts.ElementAtOrDefault(2);
            }
        }

        return new EffectToken(EffectType.DamageBonus, Magnitude: magnitude, DamageType: damageType);
    }

    /// <summary>
    /// SAVE:&lt;ability&gt;:&lt;DC&gt;
    /// e.g. SAVE:DEX:13
    /// </summary>
    private static EffectToken ParseSaveToken(string[] parts)
    {
        var ability = parts.ElementAtOrDefault(1);
        int? dc = null;
        if (parts.Length >= 3 && int.TryParse(parts[2], out var parsed))
            dc = parsed;

        return new EffectToken(EffectType.SavingThrow, AbilityOrSkill: ability, DC: dc);
    }

    public override string ToString() => Raw;
}
