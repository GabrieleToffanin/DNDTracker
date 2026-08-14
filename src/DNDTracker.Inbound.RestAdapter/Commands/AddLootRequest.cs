namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record AddLootRequest(
    string Name,
    bool IsMagicItem,
    string Notes);
