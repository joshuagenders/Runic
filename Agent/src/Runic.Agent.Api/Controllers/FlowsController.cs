using Microsoft.AspNetCore.Mvc;

namespace Runic.Agent.Api.Controllers
{
    [Route("api/[controller]")]
    public class FlowsController : Controller
    {
        // GET api/
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
