using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record SkillProficiencies(IReadOnlyList<SkillType> Proficient)
{
    public static readonly SkillProficiencies None = new([]);

    public bool IsProficient(SkillType skill) => Proficient.Contains(skill);
}
