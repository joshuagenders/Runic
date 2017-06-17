using Runic.Agent.Services;
using Runic.Agent.ThreadManagement;
using Runic.Framework.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Messaging
{
    public class HandlerRegistry
    {
        private readonly IMessagingService _messagingService;
        private readonly IThreadOrchestrator _threadOrchestrator;

        public HandlerRegistry(IMessagingService messagingService, IThreadOrchestrator threadOrchestrator)
        {
            _messagingService = messagingService;
            _threadOrchestrator = threadOrchestrator;

        }

        public void RegisterMessageHandlers(CancellationToken ct = default(CancellationToken))
        {
            var gradualFlowService = new GradualFlowService(_threadOrchestrator);
            var constantFlowService = new ConstantFlowService(_threadOrchestrator);
            var graphFlowService = new GraphFlowService(_threadOrchestrator);

            Func<GradualFlowExecutionRequest, Task> gradualHandler =
                async request => await Task.Run(() => gradualFlowService.ExecuteFlow(request, ct), ct);

            Func<ConstantFlowExecutionRequest, Task> constantHandler =
                async request => await Task.Run(() => constantFlowService.ExecuteFlow(request, ct), ct);

            Func<GraphFlowExecutionRequest, Task> graphHandler =
                async request => await Task.Run(() => graphFlowService.ExecuteFlow(request, ct), ct);

            _messagingService.RegisterMessageHandler(gradualHandler);
            _messagingService.RegisterMessageHandler(constantHandler);
            _messagingService.RegisterMessageHandler(graphHandler);
        }
    }
}
