using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record SavingThrowProficiencies(IReadOnlyList<AbilityType> Proficient)
{
    public static readonly SavingThrowProficiencies None = new([]);

    public bool IsProficient(AbilityType ability) => Proficient.Contains(ability);
}
