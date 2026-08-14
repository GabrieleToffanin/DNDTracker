using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class AddNpcCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<AddNpcCommand>
{
    public async Task Handle(AddNpcCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.AddNpc(request.Npc);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
