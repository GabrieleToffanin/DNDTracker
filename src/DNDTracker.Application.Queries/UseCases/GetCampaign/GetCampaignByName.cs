using DNDTracker.Application.Responses;
using DNDTracker.SharedKernel.Queries;

namespace DNDTracker.Application.Queries.UseCases.GetCampaign;

public record GetCampaignByName(string CampaignName) : IQuery<CampaignDto>;