using Microsoft.AspNetCore.Mvc;
using Runic.Agent.Api.Messaging;
using Runic.Framework.Models;
using System.Threading.Tasks;

namespace Runic.Agent.Api.Controllers
{
    [Route("api/[controller]")]
    public class FlowsController : Controller
    {
        private readonly IMessagingClient _client;
        public FlowsController(IMessagingClient client)
        {
            _client = client;
        }

        //add update flow
        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] Flow flow)
        {
            await _client.PublishMessageAsync(new AddUpdateFlowRequest()
            {
                Flow = flow
            });
            return Created($"/api/flows/{flow.Name}", flow);
        }

        [HttpGet("running")]
        public IActionResult GetRunningFlows()
        {

            return Ok();
        }

        [HttpPut()]
        public async Task<IActionResult> StopAll([FromBody] StopAllFlowsRequest request)
        {
            await _client.PublishMessageAsync(request);
            return Ok();
        }
    }
}
