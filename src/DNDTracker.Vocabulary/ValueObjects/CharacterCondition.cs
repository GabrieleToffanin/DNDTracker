namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record CharacterCondition(
    string Name,
    int? RemainingRounds,
    EffectCode? EffectCode = null);
