using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class ReorderCombatCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<ReorderCombatCommand>
{
    public async Task Handle(ReorderCombatCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.ReorderCombat(request.CombatantId, request.TargetIndex);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
