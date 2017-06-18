using Runic.Agent.Services;
using Runic.Agent.ThreadManagement;
using Runic.Framework.Models;
using System.Threading;

namespace Runic.Agent.Messaging
{
    public class HandlerRegistry : IHandlerRegistry
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

            _messagingService.RegisterMessageHandler<GradualFlowExecutionRequest>(
                (request) => gradualFlowService.ExecuteFlow(request, ct));
            _messagingService.RegisterMessageHandler<ConstantFlowExecutionRequest>(
                (request) => constantFlowService.ExecuteFlow(request, ct));
            _messagingService.RegisterMessageHandler<GraphFlowExecutionRequest>(
                (request) => graphFlowService.ExecuteFlow(request, ct));
            _messagingService.RegisterMessageHandler<SetThreadLevelRequest>(
                (request) => _threadOrchestrator.SetThreadLevelAsync(request, ct));
        }
    }
}
