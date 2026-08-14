using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class AddLootCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<AddLootCommand>
{
    public async Task Handle(AddLootCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.AddLoot(request.Loot);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
