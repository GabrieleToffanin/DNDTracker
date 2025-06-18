using DNDTracker.SharedKernel;
using DNDTracker.SharedKernel.Queries;

namespace DNDTracker.Application.Queries.UseCases.GetCampaign;

public record GetAllCampaigns : IQuery<IEnumerable<CampaignDto>>;