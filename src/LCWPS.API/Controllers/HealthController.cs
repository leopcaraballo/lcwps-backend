using Microsoft.AspNetCore.Mvc;

namespace LCWPS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "ok", service = "lcwps-backend" });
    }
}
