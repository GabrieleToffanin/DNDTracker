using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel;
using DNDTracker.SharedKernel.Queries;

namespace DNDTracker.Application.Queries.UseCases.GetCampaign;

public class GetAllCampaignsHandler(
    ICampaignRepository campaignRepository) : IQueryHandler<GetAllCampaigns, IEnumerable<CampaignDto>>
{
    public async Task<IEnumerable<CampaignDto>> Handle(GetAllCampaigns request, CancellationToken cancellationToken)
    {
        var campaigns = await campaignRepository.GetAllCampaignsAsync(cancellationToken);
        
        return campaigns.Select(MapToDto);
    }

    private static CampaignDto MapToDto(Campaign campaign)
    {
        return new CampaignDto(
            campaign.CampaignName,
            campaign.CampaignDescription);
    }
}