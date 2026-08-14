namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record SessionLogEntry(
    Guid Id,
    DateTime Date,
    int DurationMinutes,
    string Summary,
    string DungeonMasterNotes);
