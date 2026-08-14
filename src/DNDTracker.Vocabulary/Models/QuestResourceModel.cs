using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class QuestResourceModel
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public string Title { get; set; } = string.Empty;
    public QuestStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;
    public CampaignModel Campaign { get; set; } = null!;

    public static QuestResourceModel From(QuestResource quest) => new()
    {
        Id = quest.Id,
        Title = quest.Title,
        Status = quest.Status,
        Description = quest.Description
    };

    public QuestResource ToValueObject() => new(Id, Title, Status, Description);

    public void Apply(QuestResourceModel source)
    {
        Title = source.Title;
        Status = source.Status;
        Description = source.Description;
    }
}
