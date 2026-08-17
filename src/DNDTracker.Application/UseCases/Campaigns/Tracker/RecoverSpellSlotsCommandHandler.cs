using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class RecoverSpellSlotsCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<RecoverSpellSlotsCommand>
{
    public async Task Handle(RecoverSpellSlotsCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        var hero = campaign.Heroes.FirstOrDefault(h => h.Id.Id == request.HeroId)
            ?? throw new CharacterNotFoundException($"Hero {request.HeroId} not found.");

        hero.RecoverSpellSlots(request.RestType);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
