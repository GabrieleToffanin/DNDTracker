using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class AddCombatConditionCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<AddCombatConditionCommand>
{
    public async Task Handle(AddCombatConditionCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.AddCombatCondition(request.CombatantId, request.Condition);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
