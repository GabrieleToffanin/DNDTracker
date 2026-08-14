namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record AddCombatConditionRequest(
    Guid CombatantId,
    string Condition,
    int? RemainingRounds);
