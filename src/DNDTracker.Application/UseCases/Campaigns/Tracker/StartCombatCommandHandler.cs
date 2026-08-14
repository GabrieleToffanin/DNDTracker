using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class StartCombatCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<StartCombatCommand>
{
    public async Task Handle(StartCombatCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.StartCombat(request.Combatants);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
