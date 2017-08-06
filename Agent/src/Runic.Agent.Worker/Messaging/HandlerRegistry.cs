using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Framework.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Worker.Messaging
{
    public class HandlerRegistry : IHandlerRegistry
    {
        private readonly IMessagingService _messagingService;
        private readonly IPatternService _patternService;
        private readonly IThreadManager _threadManager;
        private readonly IDatetimeService _datetimeService;

        public HandlerRegistry(IMessagingService messagingService, IPatternService threadOrchestrator, IThreadManager threadManager, IDatetimeService datetimeService)
        {
            _messagingService = messagingService;
            _patternService = threadOrchestrator;
            _threadManager = threadManager;
            _datetimeService = datetimeService;
        }

        public void RegisterMessageHandlers(CancellationToken ctx)
        {
            RegisterConstantMessageHandler(ctx);
            RegisterGraphMessageHandler(ctx);
            RegisterGradualMessageHandler(ctx);
            RegisterThreadLevelMessageHandler(ctx);
        }

        private void RegisterThreadLevelMessageHandler(CancellationToken ctx)
        {
            _messagingService.RegisterMessageHandler(
                async (SetThreadLevelRequest request) => await _threadManager.SetThreadLevelAsync(request, ctx));
        }

        private void RegisterConstantMessageHandler(CancellationToken ctx)
        {
            _messagingService.RegisterMessageHandler(
                (ConstantFlowExecutionRequest request) =>
                {
                    var pattern = new ConstantThreadPattern(_datetimeService)
                    {
                        ThreadCount = request.ThreadPattern.ThreadCount,
                        DurationSeconds = request.ThreadPattern.DurationSeconds
                    };
                    _patternService.StartThreadPattern(request.PatternExecutionId, request.Flow, pattern, ctx);
                    return Task.CompletedTask;
                });
        }

        private void RegisterGraphMessageHandler(CancellationToken ctx)
        {
            _messagingService.RegisterMessageHandler(
                (GraphFlowExecutionRequest request) =>
                {
                    var pattern = new GraphThreadPattern(_datetimeService)
                    {
                        DurationSeconds = request.ThreadPattern.DurationSeconds,
                        Points = request.ThreadPattern.Points
                    };
                    _patternService.StartThreadPattern(request.PatternExecutionId, request.Flow, pattern, ctx);
                    return Task.CompletedTask;
                });
        }

        private void RegisterGradualMessageHandler(CancellationToken ctx)
        {
            _messagingService.RegisterMessageHandler(
                (GradualFlowExecutionRequest request) =>
                {
                    var pattern = new GradualThreadPattern(_datetimeService)
                    {
                        DurationSeconds = request.ThreadPattern.DurationSeconds,
                        Points = request.ThreadPattern.Points,
                        RampDownSeconds = request.ThreadPattern.RampDownSeconds,
                        RampUpSeconds = request.ThreadPattern.RampUpSeconds,
                        StepIntervalSeconds = request.ThreadPattern.StepIntervalSeconds,
                        ThreadCount = request.ThreadPattern.ThreadCount
                    };

                    _patternService.StartThreadPattern(request.PatternExecutionId, request.Flow, pattern, ctx);
                    return Task.CompletedTask;
                });
        }
    }
}
