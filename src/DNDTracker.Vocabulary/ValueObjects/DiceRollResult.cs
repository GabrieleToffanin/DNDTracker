namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record DiceRollResult(
    string Expression,
    int Total,
    IReadOnlyCollection<int> Rolls,
    int Modifier,
    string? Context);
