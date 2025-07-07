using DNDTracker.Domain;
using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.AddHero;

public class AddHeroToCampaignCommandHandler(
    IEventPublisher eventPublisher,
    ICampaignRepository campaignRepository)
    : ICommandHandler<AddHeroToCampaignCommand>
{
    public async Task Handle(AddHeroToCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository
            .GetCampaignAsync(request.CampaignName, cancellationToken);
        
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        campaign.AddHero(request.Hero);
        
        foreach(var domainEvent in campaign.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}