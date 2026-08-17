using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Vocabulary.Models;

public class HeroSkillProficiencyModel
{
    public Guid Id { get; set; }
    public Guid HeroId { get; set; }
    public SkillType Skill { get; set; }
    public HeroModel Hero { get; set; } = null!;

    public static HeroSkillProficiencyModel From(Guid heroId, SkillType skill) => new()
    {
        Id = Guid.NewGuid(),
        HeroId = heroId,
        Skill = skill
    };
}
