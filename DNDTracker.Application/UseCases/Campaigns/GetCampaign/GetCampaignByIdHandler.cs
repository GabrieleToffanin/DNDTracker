using System.Runtime.CompilerServices;
using DNDTracker.Application.Abstractions;
using DNDTracker.Application.Responses;
using DNDTracker.Domain.Entities;
using DNDTracker.SharedKernel.Queries;

namespace DNDTracker.Application.UseCases.Campaigns.GetCampaign;

public class GetCampaignByIdHandler(
    ICampaignRepository campaignRepository) : IQueryHandler<GetCampaignByName, CampaignDto>
{
    public async Task<CampaignDto> Handle(GetCampaignByName request, CancellationToken cancellationToken)
    {
        // Simulate fetching the campaign using the CampaignId from the request.
        // You might replace this with a real database or repository call in your implementation.
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName);

        this.ThrowIfCampaignNotFound(request.CampaignName, campaign);
        
        return this.MapToDto(campaign);
    }

    private CampaignDto MapToDto(Campaign campaign)
    {
        return new CampaignDto(
            campaign.CampaignName,
            campaign.CampaignDescription);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIfCampaignNotFound(string campaignName, Campaign campaign)
    {
        if (campaign is null)
        {
            throw new KeyNotFoundException($"Campaign with ID {campaignName} was not found.");
        }
    }
}