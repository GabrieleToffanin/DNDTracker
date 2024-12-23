using DNDTracker.Domain.Entities;
using DNDTracker.SharedKernel.Queries;

namespace DNDTracker.Application.UseCases.Campaigns.GetCampaign;

public class GetCampaignByIdHandler : IQueryHandler<GetCampaignById, Campaign>
{
    public async Task<Campaign> Handle(GetCampaignById request, CancellationToken cancellationToken)
    {
        return default;
    }
}