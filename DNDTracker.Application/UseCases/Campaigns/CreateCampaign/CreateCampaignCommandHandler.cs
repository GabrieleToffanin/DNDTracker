using DNDTracker.Domain.Abstractions;
using DNDTracker.Domain.Entities;
using DNDTracker.SharedKernel.Commands;

namespace DNDTracker.Application.UseCases.Campaigns.CreateCampaign;

public sealed class CreateCampaignCommandHandler(
    ICampaignRepository campaignRepository) : ICommandHandler<CreateCampaignCommand>
{
    public async Task Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
    {
        Campaign toBeCreated = Campaign.Create(
            request.CampaignName,
            request.CampaignDescription,
            request.CampaignImage,
            request.CreatedDate,
            request.IsActive);

        await campaignRepository.CreateCampaignAsync(toBeCreated, cancellationToken);
    }
}