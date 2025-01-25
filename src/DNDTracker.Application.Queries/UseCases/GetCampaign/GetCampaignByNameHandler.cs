using System.Runtime.CompilerServices;
using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel;
using DNDTracker.SharedKernel.Queries;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.Queries.UseCases.GetCampaign;

public class GetCampaignByNameHandler(
    ICampaignRepository campaignRepository) : IQueryHandler<GetCampaignByName, CampaignDto>
{
    public async Task<CampaignDto> Handle(GetCampaignByName request, CancellationToken cancellationToken)
    {
        // Simulate fetching the campaign using the CampaignId from the request.
        // You might replace this with a real database or repository call in your implementation.
        var campaign = await campaignRepository.GetCampaignAsync(
            request.CampaignName,
            cancellationToken);

        this.ThrowIfCampaignNotFound(request.CampaignName, campaign);
        
        // If we reach this, for sure campaign is not null.
        return this.MapToDto(campaign!);
    }

    private CampaignDto MapToDto(Campaign campaign)
    {
        return new CampaignDto(
            campaign.CampaignName,
            campaign.CampaignDescription);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIfCampaignNotFound(string campaignName, Campaign? campaign)
    {
        if (campaign is null)
        {
            throw new CampaignNotFoundException($"Campaigns with Name {campaignName} was not found.");
        }
    }
}