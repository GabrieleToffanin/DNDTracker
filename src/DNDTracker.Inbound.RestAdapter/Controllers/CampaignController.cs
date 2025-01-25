using DNDTracker.Application.Responses;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Application.UseCases.Campaigns.GetCampaign;
using DNDTracker.Inbound.RestAdapter.Commands;
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
        GetCampaignByName getByName = new(query.CampaignName);
        
        CampaignDto campaign = await mediator.Send(getByName, cancellationToken);
        
        return Ok(campaign);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(void))]
    public async Task<IActionResult> Create(
        [FromBody]CreateCampaignRequest command,
        CancellationToken cancellationToken)
    {
        var mappedRequest = new CreateCampaignCommand(
            command.CampaignName,
            command.CampaignDescription,
            command.CampaignImage,
            DateTime.Now);
        
        // send the command to the mediator to handle
        await mediator.Send(
            mappedRequest,
            cancellationToken);

        // if successful, return status code 201 (Created) with campaign data
        return CreatedAtAction(nameof(Get), new { campaignName = command.CampaignName }, null);
    }
    
}