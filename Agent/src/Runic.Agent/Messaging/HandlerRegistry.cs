using Runic.Agent.ThreadManagement;
using Runic.Agent.ThreadPatterns;
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
            RegisterConstantMessageHandler();
            RegisterGraphMessageHandler();
            RegisterGradualMessageHandler();
            RegisterThreadLevelMessageHandler(ct);
        }

        private void RegisterThreadLevelMessageHandler(CancellationToken ct = default(CancellationToken))
        {
            _messagingService.RegisterMessageHandler<SetThreadLevelRequest>(
                (request) => _threadOrchestrator.SetThreadLevelAsync(request, ct));
        }

        private void RegisterConstantMessageHandler()
        {
            _messagingService.RegisterMessageHandler<ConstantFlowExecutionRequest>(
                (request) =>
                {
                    var pattern = new ConstantThreadPattern()
                    {
                        ThreadCount = request.ThreadPattern.ThreadCount,
                        DurationSeconds = request.ThreadPattern.DurationSeconds
                    };

                    _threadOrchestrator.AddNewPattern(request.PatternExecutionId, request.Flow, pattern);
                });
        }

        private void RegisterGraphMessageHandler()
        {
            _messagingService.RegisterMessageHandler<GraphFlowExecutionRequest>(
                (request) =>
                {
                    var pattern = new GraphThreadPattern()
                    {
                        DurationSeconds = request.ThreadPattern.DurationSeconds,
                        Points = request.ThreadPattern.Points
                    };
                    _threadOrchestrator.AddNewPattern(request.PatternExecutionId, request.Flow, pattern);
                });
        }

        private void RegisterGradualMessageHandler()
        {
            _messagingService.RegisterMessageHandler<GradualFlowExecutionRequest>(
                (request) =>
                {
                    var pattern = new GradualThreadPattern()
                    {
                        DurationSeconds = request.ThreadPattern.DurationSeconds,
                        Points = request.ThreadPattern.Points,
                        RampDownSeconds = request.ThreadPattern.RampDownSeconds,
                        RampUpSeconds = request.ThreadPattern.RampUpSeconds,
                        StepIntervalSeconds = request.ThreadPattern.StepIntervalSeconds,
                        ThreadCount = request.ThreadPattern.ThreadCount
                    };

                    _threadOrchestrator.AddNewPattern(request.PatternExecutionId, request.Flow, pattern);
                });
        }
    }
}
