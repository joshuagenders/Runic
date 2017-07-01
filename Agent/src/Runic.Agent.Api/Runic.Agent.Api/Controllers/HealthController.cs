using Microsoft.AspNetCore.Mvc;

namespace Runic.Agent.Api.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        // GET api/health
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
