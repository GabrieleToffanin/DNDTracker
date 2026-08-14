using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class HeroConditionModel
{
    public Guid Id { get; set; }
    public Guid HeroId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? RemainingRounds { get; set; }
    public HeroModel Hero { get; set; } = null!;

    public static HeroConditionModel From(CharacterCondition condition) => new()
    {
        Id = Guid.NewGuid(),
        Name = condition.Name,
        RemainingRounds = condition.RemainingRounds
    };

    public CharacterCondition ToValueObject() => new(Name, RemainingRounds);

    public void Apply(HeroConditionModel source)
    {
        Name = source.Name;
        RemainingRounds = source.RemainingRounds;
    }
}
