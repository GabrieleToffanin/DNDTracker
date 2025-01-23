using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Inbound.RestAdapter.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Healthy");
    }
    
}