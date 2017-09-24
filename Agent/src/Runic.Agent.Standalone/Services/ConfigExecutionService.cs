using Microsoft.Extensions.Logging;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Providers;
using Runic.Agent.Framework.Models;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.TestHarness.Services;

namespace Runic.Agent.Standalone.Services
{
    public class ConfigExecutionService
    {
        private readonly IPopulationPatternController _patternController;
        private Journey _flow;
        private readonly IAgentConfig _config;
        private readonly IFlowProvider _flowProvider;
        private readonly ILogger _logger;
        private readonly IDatetimeService _datetimeService;

        public ConfigExecutionService(
            IPopulationPatternController patternController, 
            IAgentConfig config, 
            IFlowProvider flowProvider, 
            ILoggerFactory loggerFactory, 
            IDatetimeService datetimeService)
        {
            _patternController = patternController;
            _config = config;
            _flowProvider = flowProvider;
            _logger = loggerFactory.CreateLogger<ConfigExecutionService>();
            _datetimeService = datetimeService;
        }

        public async Task StartThreadPattern(CancellationToken ctx = default(CancellationToken))
        {
            //todo standardise tokens
            ImportFlow();
            switch (_config.AgentSettings.FlowThreadPatternName.ToLowerInvariant())
            {
                case "graph":
                    StartGraphPattern(ctx);
                    break;
                case "gradual":
                    StartGradualPattern(ctx);
                    break;
                case "constant":
                    StartConstantPattern(ctx);
                    break;
                default:
                    throw new ThreadPatternNotRecognisedException();
            }
            await Task.CompletedTask;
        }

        private void ImportFlow()
        {
            _flow = _flowProvider.GetFlow(_config.AgentSettings.AgentFlowFilepath);
            _logger.LogInformation($"Importing flow {_flow.Name}.");
            _logger.LogInformation($"{_flow.Name} imported.");
        }

        private void StartConstantPattern(CancellationToken ctx = default(CancellationToken))
        {
            var pattern = new ConstantPopulationPattern(_datetimeService)
                {
                    PersonCount = _config.AgentSettings.FlowThreadCount,
                    DurationSeconds = _config.AgentSettings.FlowDurationSeconds
                };
            _logger.LogInformation($"Starting constant thread pattern {_config.AgentSettings.FlowPatternExecutionId}");
            _patternController.AddPopulationPattern(_config.AgentSettings.FlowPatternExecutionId, _flow, pattern, ctx);
        }

        private void StartGraphPattern(CancellationToken ctx = default(CancellationToken))
        {
            var pattern = new GraphPopulationPattern(_datetimeService)
            {
                DurationSeconds = _config.AgentSettings.FlowDurationSeconds,
                Points = _config.AgentSettings.FlowPoints.ParsePoints()
            };
            _logger.LogInformation($"Starting graph thread pattern {_config.AgentSettings.FlowPatternExecutionId}");
            _patternController.AddPopulationPattern(_config.AgentSettings.FlowPatternExecutionId, _flow, pattern, ctx);
        }

        private void StartGradualPattern(CancellationToken ctx = default(CancellationToken))
        {
            var pattern = new GradualPopulationPattern(_datetimeService)
            {
                DurationSeconds = _config.AgentSettings.FlowDurationSeconds,
                RampDownSeconds = _config.AgentSettings.FlowRampDownSeconds,
                RampUpSeconds = _config.AgentSettings.FlowRampUpSeconds,
                ThreadCount = _config.AgentSettings.FlowThreadCount
            };
            _logger.LogInformation($"Starting gradual thread pattern {_config.AgentSettings.FlowPatternExecutionId}");
            _patternController.AddPopulationPattern(_config.AgentSettings.FlowPatternExecutionId, _flow, pattern, ctx);
        }
    }
}
