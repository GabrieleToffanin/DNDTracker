using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Vocabulary.Models;

public class HeroSavingThrowProficiencyModel
{
    public Guid Id { get; set; }
    public Guid HeroId { get; set; }
    public AbilityType Ability { get; set; }
    public HeroModel Hero { get; set; } = null!;

    public static HeroSavingThrowProficiencyModel From(Guid heroId, AbilityType ability) => new()
    {
        Id = Guid.NewGuid(),
        HeroId = heroId,
        Ability = ability
    };
}
