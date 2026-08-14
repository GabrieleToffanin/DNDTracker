namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record CombatState(
    int Round,
    int TurnIndex,
    IReadOnlyCollection<CombatantState> InitiativeOrder);
