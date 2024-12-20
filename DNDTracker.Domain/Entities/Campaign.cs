using DNDTracker.Domain.Common;
using DNDTracker.Domain.ValueObjects;

namespace DNDTracker.Domain.Entities;



public sealed class Campaign : AggregateRoot<CampaignId>
{
    private Campaign(
        CampaignId id,
        string campaignName,
        string campaignDescription,
        string campaignImage,
        bool isActive,
        DateTime createdDate,
        DateTime updatedDate,
        DateTime? deletedDate) : base(id)
    {
        CampaignName = campaignName;
        CampaignDescription = campaignDescription;
        CampaignImage = campaignImage;
        IsActive = isActive;
        CreatedDate = createdDate;
        UpdatedDate = updatedDate;
        DeletedDate = deletedDate;
    }

    public string CampaignName { get; private set; }
    public string CampaignDescription { get; private set; }
    public string CampaignImage { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime UpdatedDate { get; private set; }
    public DateTime? DeletedDate { get; private set; }
    
    public List<Hero> Heroes { get; private set; } = [];
    
    public static Campaign Create(
        string campaignName,
        string campaignDescription,
        string campaignImage,
        bool isActive)
    {
        var id = CampaignId.Create();
        var currentDate = DateTime.UtcNow;

        return new Campaign(
            id,
            campaignName,
            campaignDescription,
            campaignImage,
            isActive,
            currentDate,
            currentDate,
            null
        );
    }
}