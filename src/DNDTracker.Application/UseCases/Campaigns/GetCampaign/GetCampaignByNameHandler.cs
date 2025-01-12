using System.Runtime.CompilerServices;
using DNDTracker.Application.Exceptions;
using DNDTracker.Application.Responses;
using DNDTracker.Domain.Abstractions;
using DNDTracker.Domain.Entities;
using DNDTracker.SharedKernel.Queries;

namespace DNDTracker.Application.UseCases.Campaigns.GetCampaign;

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
            throw new CampaignNotFoundException($"Campaign with Name {campaignName} was not found.");
        }
    }
}