using Microsoft.Extensions.Logging;
using Runic.Agent.Aws.Configuration;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Services;
using Runic.Agent.Aws.Services;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Aws.Providers;

namespace Runic.Agent.Aws
{
    public class Application : IApplication
    {
        private readonly IPatternService _patternService;
        private readonly IFlowManager _flowManager;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IAgentConfig _config;
        private readonly IFlowProvider _flowProvider;
        private readonly IDatetimeService _datetimeService;

        public Application(IPatternService patternService, IFlowManager flowManager, ILoggerFactory loggerFactory, IAgentConfig config, IFlowProvider flowProvider, IDatetimeService datetimeService)
        {
            _patternService = patternService;
            _flowManager = flowManager;
            _logger = loggerFactory.CreateLogger<IApplication>();
            _loggerFactory = loggerFactory;
            _config = config;
            _flowProvider = flowProvider;
            _datetimeService = datetimeService;
        }

        public async Task RunApplicationAsync(CancellationToken ctx = default(CancellationToken))
        {
            _logger.LogInformation("Creating execution service.");
            var executionService = new ConfigExecutionService(_patternService, _flowManager, _config, _flowProvider, _loggerFactory, _datetimeService);
            await executionService.StartThreadPattern(ctx);
            _logger.LogInformation("Thread pattern started.");
            await _patternService.GetCompletionTaskAsync(_config.AgentSettings.FlowPatternExecutionId, ctx);
            _logger.LogInformation("Thread pattern complete.");
        }
    }
}
