namespace DNDTracker.BackendInfrastructure.PostgresDb.Models;

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
}