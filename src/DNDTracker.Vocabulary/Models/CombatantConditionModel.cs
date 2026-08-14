using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class CombatantConditionModel
{
    public Guid Id { get; set; }
    public Guid CombatantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? RemainingRounds { get; set; }
    public CombatantStateModel Combatant { get; set; } = null!;

    public static CombatantConditionModel From(CharacterCondition condition) => new()
    {
        Id = Guid.NewGuid(),
        Name = condition.Name,
        RemainingRounds = condition.RemainingRounds
    };

    public CharacterCondition ToValueObject() => new(Name, RemainingRounds);
}
