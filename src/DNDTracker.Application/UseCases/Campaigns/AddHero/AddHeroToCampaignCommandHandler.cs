using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.AddHero;

public class AddHeroToCampaignCommandHandler(ICampaignRepository campaignRepository)
    : ICommandHandler<AddHeroToCampaignCommand>
{
    public async Task Handle(AddHeroToCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository
            .GetCampaignAsync(request.CampaignName, cancellationToken);
        
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.AddHero(request.Hero);

        await campaignRepository.UpdateAsync(cancellationToken);
    }
}