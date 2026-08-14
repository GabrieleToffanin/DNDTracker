using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class AdvanceCombatTurnCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<AdvanceCombatTurnCommand>
{
    public async Task Handle(AdvanceCombatTurnCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.AdvanceCombatTurn();
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
