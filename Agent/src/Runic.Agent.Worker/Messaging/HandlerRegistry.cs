using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Framework.Models;
using System.Threading;

namespace Runic.Agent.Worker.Messaging
{
    public class HandlerRegistry : IHandlerRegistry
    {
        private readonly IMessagingService _messagingService;
        private readonly IPatternService _patternService;
        private readonly IThreadManager _threadManager;

        public HandlerRegistry(IMessagingService messagingService, IPatternService threadOrchestrator, IThreadManager threadManager)
        {
            _messagingService = messagingService;
            _patternService = threadOrchestrator;
            _threadManager = threadManager;
        }

        public void RegisterMessageHandlers(CancellationToken ct = default(CancellationToken))
        {
            RegisterConstantMessageHandler(ct);
            RegisterGraphMessageHandler(ct);
            RegisterGradualMessageHandler(ct);
            RegisterThreadLevelMessageHandler(ct);
        }

        private void RegisterThreadLevelMessageHandler(CancellationToken ct = default(CancellationToken))
        {
            _messagingService.RegisterMessageHandler<SetThreadLevelRequest>(
                async (request) => await _threadManager.SetThreadLevelAsync(request, ct));
        }

        private void RegisterConstantMessageHandler(CancellationToken ct = default(CancellationToken))
        {
            _messagingService.RegisterMessageHandler<ConstantFlowExecutionRequest>(
                (request) =>
                {
                    var pattern = new ConstantThreadPattern()
                    {
                        ThreadCount = request.ThreadPattern.ThreadCount,
                        DurationSeconds = request.ThreadPattern.DurationSeconds
                    };

                    _patternService.StartThreadPattern(request.PatternExecutionId, request.Flow, pattern, ct);
                });
        }

        private void RegisterGraphMessageHandler(CancellationToken ct)
        {
            _messagingService.RegisterMessageHandler<GraphFlowExecutionRequest>(
                (request) =>
                {
                    var pattern = new GraphThreadPattern()
                    {
                        DurationSeconds = request.ThreadPattern.DurationSeconds,
                        Points = request.ThreadPattern.Points
                    };
                    _patternService.StartThreadPattern(request.PatternExecutionId, request.Flow, pattern, ct);
                });
        }

        private void RegisterGradualMessageHandler(CancellationToken ct)
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

                    _patternService.StartThreadPattern(request.PatternExecutionId, request.Flow, pattern, ct);
                });
        }
    }
}
