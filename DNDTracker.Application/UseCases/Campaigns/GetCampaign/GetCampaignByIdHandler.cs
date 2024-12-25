using System.Runtime.CompilerServices;
using DNDTracker.Application.Abstractions;
using DNDTracker.Domain.Entities;
using DNDTracker.SharedKernel.Queries;

namespace DNDTracker.Application.UseCases.Campaigns.GetCampaign;

public class GetCampaignByIdHandler(
    ICampaignRepository campaignRepository) : IQueryHandler<GetCampaignById, Campaign>
{
    public async Task<Campaign> Handle(GetCampaignById request, CancellationToken cancellationToken)
    {
        // Simulate fetching the campaign using the CampaignId from the request.
        // You might replace this with a real database or repository call in your implementation.
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignId);

        this.ThrowIfCampaignNotFound(request.CampaignId, campaign);
        
        return campaign;
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowIfCampaignNotFound(Guid campaignId, Campaign campaign)
    {
        if (campaign is null)
        {
            throw new KeyNotFoundException($"Campaign with ID {campaignId} was not found.");
        }
    }
}