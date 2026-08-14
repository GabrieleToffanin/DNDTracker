using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class AddMonsterToLibraryCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<AddMonsterToLibraryCommand>
{
    public async Task Handle(AddMonsterToLibraryCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.AddMonsterToLibrary(request.Monster);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
