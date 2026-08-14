using DNDTracker.SharedKernel;
using DNDTracker.SharedKernel.Queries;

namespace DNDTracker.Application.Queries.UseCases.GetCampaignTracker;

public sealed record GetCampaignTrackerByName(
    string CampaignName,
    Guid? ViewerUserId) : IQuery<CampaignTrackerDto>;
