using Microsoft.AspNetCore.Mvc;
using Runic.Agent.Api.Messaging;
using Runic.Framework.Models;
using System.Threading.Tasks;

namespace Runic.Agent.Api.Controllers
{
    [Route("api/[controller]")]
    public class ThreadsController : Controller
    {
        private readonly IMessagingClient _client;
        public ThreadsController(IMessagingClient client)
        {
            _client = client;
        }

        [HttpPut()]
        public async Task<IActionResult> ThreadLevel([FromBody] SetThreadLevelRequest request)
        {
            await _client.PublishMessageAsync(request);
            return Ok();
        }

        [HttpPut()]
        public async Task<IActionResult> ConstantPattern([FromBody] ConstantFlowExecutionRequest request)
        {
            await _client.PublishMessageAsync(request);
            return Ok();
        }

        [HttpPut()]
        public async Task<IActionResult> GradualPattern([FromBody] GradualFlowExecutionRequest request)
        {
            await _client.PublishMessageAsync(request);
            return Ok();
        }

        [HttpPut()]
        public async Task<IActionResult> GraphPattern([FromBody] GraphFlowExecutionRequest request)
        {
            await _client.PublishMessageAsync(request);
            return Ok();
        }

        [HttpPut()]
        public async Task<IActionResult> ClearPatterns([FromBody] ClearAllPatternsRequest request)
        {
            await _client.PublishMessageAsync(request);
            return Ok();
        }
    }
}
