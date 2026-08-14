using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Vocabulary.ValueObjects;

public sealed record QuestResource(
    Guid Id,
    string Title,
    QuestStatus Status,
    string Description);
