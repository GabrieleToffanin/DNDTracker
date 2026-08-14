namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record AddLocationRequest(
    string Name,
    string Description,
    string? MapUrl);
