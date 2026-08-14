using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Vocabulary.Models;

public class SessionLogEntryModel
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public DateTime Date { get; set; }
    public int DurationMinutes { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string DungeonMasterNotes { get; set; } = string.Empty;
    public CampaignModel Campaign { get; set; } = null!;

    public static SessionLogEntryModel From(SessionLogEntry entry) => new()
    {
        Id = entry.Id,
        Date = entry.Date,
        DurationMinutes = entry.DurationMinutes,
        Summary = entry.Summary,
        DungeonMasterNotes = entry.DungeonMasterNotes
    };

    public SessionLogEntry ToValueObject() => new(Id, Date, DurationMinutes, Summary, DungeonMasterNotes);

    public void Apply(SessionLogEntryModel source)
    {
        Date = source.Date;
        DurationMinutes = source.DurationMinutes;
        Summary = source.Summary;
        DungeonMasterNotes = source.DungeonMasterNotes;
    }
}
