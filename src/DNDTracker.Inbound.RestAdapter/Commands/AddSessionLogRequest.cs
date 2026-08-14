namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record AddSessionLogRequest(
    DateTime Date,
    int DurationMinutes,
    string Summary,
    string DungeonMasterNotes);
