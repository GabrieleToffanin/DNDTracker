using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class UpdateCombatantHitPointsCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<UpdateCombatantHitPointsCommand>
{
    public async Task Handle(UpdateCombatantHitPointsCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.ApplyCombatantHitPointDelta(request.CombatantId, request.Damage, request.Healing, request.TemporaryHitPointsDelta);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
