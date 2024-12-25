using DNDTracker.Application.UseCases.Campaigns.GetCampaign;
using DNDTracker.Domain.Entities;
using DNDTracker.Presentation.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController(
    IMediator mediator) : ControllerBase
{
    [HttpGet("{campaignId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(GetCampaignQuery query)
    {
        // Search for the specific campaign based on received guid
        GetCampaignById getById = new(query.CampaignId);
        
        Campaign campaign = await mediator.Send(getById);
        
        return Ok(campaign);
    }
}