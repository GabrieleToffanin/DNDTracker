namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record ReorderCombatRequest(
    Guid CombatantId,
    int TargetIndex);
