using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class UpdateCharacterHitPointsCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<UpdateCharacterHitPointsCommand>
{
    public async Task Handle(UpdateCharacterHitPointsCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        var hero = campaign.Heroes.FirstOrDefault(h => h.Id.Id == request.CharacterId)
            ?? throw new CharacterNotFoundException($"Character {request.CharacterId} not found.");

        hero.ApplyHitPointDelta(request.Damage, request.Healing, request.TemporaryHitPointsDelta);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
