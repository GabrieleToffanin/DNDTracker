using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class AddCampaignMemberCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<AddCampaignMemberCommand>
{
    public async Task Handle(AddCampaignMemberCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.AddMember(request.Member);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
