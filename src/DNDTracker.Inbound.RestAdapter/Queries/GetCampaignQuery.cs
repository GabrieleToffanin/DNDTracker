using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Inbound.RestAdapter.Queries;

public class GetCampaignQuery
{
    [FromQuery] public required string CampaignName { get; set; }
}