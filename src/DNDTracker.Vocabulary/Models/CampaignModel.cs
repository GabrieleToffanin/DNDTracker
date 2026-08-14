namespace DNDTracker.Vocabulary.Models;

public class CampaignModel
{
    public Guid Id { get; set; }
    public string CampaignName { get; set; }
    public string CampaignDescription { get; set; }
    public string CampaignImage { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string MonsterLibraryJson { get; set; } = "[]";
    public string? ActiveCombatJson { get; set; }
    public string SessionLogsJson { get; set; } = "[]";
    public string TimelineEntriesJson { get; set; } = "[]";
    public string NpcsJson { get; set; } = "[]";
    public string LocationsJson { get; set; } = "[]";
    public string QuestsJson { get; set; } = "[]";
    public string LootJson { get; set; } = "[]";
    public string MembersJson { get; set; } = "[]";
    public List<HeroModel> Heroes { get; private set; } = [];
}
