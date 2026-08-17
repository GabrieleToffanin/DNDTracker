using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class ApplyEffectCodeCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<ApplyEffectCodeCommand>
{
    public async Task Handle(ApplyEffectCodeCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        var hero = campaign.Heroes.FirstOrDefault(h => h.Id.Id == request.HeroId)
            ?? throw new CharacterNotFoundException($"Hero {request.HeroId} not found.");

        var effectCode = new EffectCode(request.RawEffectCode);
        var condition = new CharacterCondition(
            Name: $"Effect: {request.RawEffectCode}",
            RemainingRounds: request.DurationRounds,
            EffectCode: effectCode);

        hero.AddCondition(condition);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
