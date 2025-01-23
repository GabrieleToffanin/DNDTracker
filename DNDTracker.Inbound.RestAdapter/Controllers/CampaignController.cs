using DNDTracker.Application.Responses;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Application.UseCases.Campaigns.GetCampaign;
using DNDTracker.Inbound.RestAdapter.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Inbound.RestAdapter.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController(
    IMediator mediator) : ControllerBase
{
    [HttpGet("{campaignName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CampaignDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        GetCampaignQuery query,
        CancellationToken cancellationToken)
    {
        // Search for the specific campaign based on received guid
        GetCampaignByName getByName = new(query.CampaignName);
        
        CampaignDto campaign = await mediator.Send(getByName, cancellationToken);
        
        return Ok(campaign);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(void))]
    public async Task<IActionResult> Create(
        [FromBody]CreateCampaignCommand command,
        CancellationToken cancellationToken)
    {
        // send the command to the mediator to handle
        await mediator.Send(command, cancellationToken);

        // if successful, return status code 201 (Created) with campaign data
        return CreatedAtAction(nameof(Get), new { campaignName = command.CampaignName }, null);
    }
    
}