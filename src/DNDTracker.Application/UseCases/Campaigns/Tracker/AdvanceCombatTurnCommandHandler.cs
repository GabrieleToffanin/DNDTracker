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

        // Tick conditions only for the hero whose turn is currently ending, so each hero
        // receives exactly one tick per full round (not once per every other combatant's turn).
        if (campaign.ActiveCombat is not null)
        {
            var currentCombatant = campaign.ActiveCombat.InitiativeOrder
                .ElementAtOrDefault(campaign.ActiveCombat.TurnIndex);

            if (currentCombatant is not null)
            {
                var hero = campaign.Heroes.FirstOrDefault(h => h.Id.Id == currentCombatant.Id);
                if (hero is not null)
                    EffectResolver.TickHeroConditions(hero);
            }
        }

        campaign.AdvanceCombatTurn();
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
