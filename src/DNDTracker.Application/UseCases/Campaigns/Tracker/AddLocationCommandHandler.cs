using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class AddLocationCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<AddLocationCommand>
{
    public async Task Handle(AddLocationCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.AddLocation(request.Location);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
