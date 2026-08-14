namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record AddNpcRequest(
    string Name,
    string Role,
    string Notes);
