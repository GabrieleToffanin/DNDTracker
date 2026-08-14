namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record UpdateCharacterHitPointsRequest(
    int Damage,
    int Healing,
    int TemporaryHitPointsDelta);
