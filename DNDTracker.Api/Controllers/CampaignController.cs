using DNDTracker.Api.Queries;
using DNDTracker.Application.UseCases.Campaigns.GetCampaign;
using DNDTracker.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController(
    IMediator mediator) : ControllerBase
{
    [HttpGet("{campaignName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(GetCampaignQuery query)
    {
        // Search for the specific campaign based on received guid
        GetCampaignByName getByName = new(query.CampaignName);
        
        Campaign campaign = await mediator.Send(getByName);
        
        return Ok(campaign);
    }
}