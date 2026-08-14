using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class AddSessionLogCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<AddSessionLogCommand>
{
    public async Task Handle(AddSessionLogCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.AddSessionLog(request.SessionLog);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
