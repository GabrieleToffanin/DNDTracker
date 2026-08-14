using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Inbound.RestAdapter.Commands;

public sealed record AddQuestRequest(
    Guid? Id,
    string Title,
    QuestStatus Status,
    string Description);
