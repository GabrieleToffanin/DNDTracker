using DNDTracker.Domain.Campaigns;
using DNDTracker.Domain.Services;
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

        // Apply per-turn condition ticks (Heal/Damage from EffectCode, round countdown)
        // to hero characters whose turn is ending.
        foreach (var hero in campaign.Heroes)
            EffectResolver.TickHeroConditions(hero);

        campaign.AdvanceCombatTurn();
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
