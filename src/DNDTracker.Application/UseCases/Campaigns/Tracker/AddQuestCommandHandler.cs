using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class AddQuestCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<AddQuestCommand>
{
    public async Task Handle(AddQuestCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.AddQuest(request.Quest);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
