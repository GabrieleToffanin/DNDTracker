namespace DNDTracker.Blazor;

public sealed record ExecutableAttack(string Label, int Modifier, string MonsterName);

public sealed record RollChatEntry(
    DateTime Timestamp,
    string Actor,
    string Action,
    int DieRoll,
    int Modifier,
    int Total);
