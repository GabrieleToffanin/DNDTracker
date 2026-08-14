namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record UpdateCombatHitPointsRequest(
    Guid CombatantId,
    int Damage,
    int Healing,
    int TemporaryHitPointsDelta);
