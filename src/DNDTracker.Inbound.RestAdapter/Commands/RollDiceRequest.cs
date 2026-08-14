namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record RollDiceRequest(
    string Expression,
    int Modifier,
    string? Context);
