using DNDTracker.DataAccessObject.Mapping.HeroMap;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Vocabulary.Models;

namespace DNDTracker.DataAccessObject.Mapping.CampaignMap;

public static class CampaignModelMapping
{
    public static Campaign MapToDomain(this CampaignModel campaignModel)
    {
        return Campaign.Create(
            campaignModel.Id,
            campaignModel.CampaignName,
            campaignModel.CampaignDescription,
            campaignModel.CampaignImage,
            campaignModel.CreatedDate,
            campaignModel.IsActive,
            campaignModel.Heroes.Select(h => h.MapToDomain()).ToList()
        );
    }
    
    public static CampaignModel MapToModel(this Campaign campaign)
    {
        var campaignModel = new CampaignModel
        {
            Id = campaign.Id.Id,
            CampaignName = campaign.CampaignName,
            CampaignDescription = campaign.CampaignDescription,
            CampaignImage = campaign.CampaignImage,
            CreatedDate = campaign.CreatedDate,
            IsActive = campaign.IsActive
        };
        
        campaignModel.Heroes.AddRange(campaign.Heroes.Select(h => h.MapToModel()));
        
        return campaignModel;
    }
}