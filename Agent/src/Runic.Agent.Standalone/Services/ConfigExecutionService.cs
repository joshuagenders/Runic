using Newtonsoft.Json;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Agent.Standalone.Configuration;
using Runic.Framework.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone.Services
{
    public class ConfigExecutionService
    {
        private readonly IPatternService _patternService;
        private readonly IFlowManager _flowManager;
        private Flow _flow;
        private IAgentConfig _config;

        public ConfigExecutionService(IPatternService patternService, IFlowManager flowManager, IAgentConfig config)
        {
            _patternService = patternService;
            _flowManager = flowManager;
            _config = config;
        }

        public async Task StartThreadPattern(CancellationToken ct = default(CancellationToken))
        {
            //todo standardise tokens
            ImportFlow();
            switch (_config.AgentSettings.FlowThreadPatternName.ToLowerInvariant())
            {
                case "graph":
                    StartGraphPattern(ct);
                    break;
                case "gradual":
                    StartGradualPattern(ct);
                    break;
                case "constant":
                    StartConstantPattern(ct);
                    break;
                default:
                    throw new ThreadPatternNotRecognisedException();
            }
            await Task.CompletedTask;
        }

        private void ImportFlow()
        {
            if (!File.Exists(_config.AgentSettings.AgentFlowFilepath))
            {
                throw new FileNotFoundException($"Flow not found at {_config.AgentSettings.AgentFlowFilepath}");
            }
            _flow = JsonConvert.DeserializeObject<Flow>(File.ReadAllText(_config.AgentSettings.AgentFlowFilepath));
            _flowManager.AddUpdateFlow(_flow);
        }

        private void StartConstantPattern(CancellationToken ct = default(CancellationToken))
        {
            var pattern = new ConstantThreadPattern()
                {
                    ThreadCount = _config.AgentSettings.FlowThreadCount,
                    DurationSeconds = _config.AgentSettings.FlowDurationSeconds
                };

            _patternService.StartThreadPattern(_config.AgentSettings.FlowPatternExecutionId, _flow, pattern, ct);
        }

        private void StartGraphPattern(CancellationToken ct)
        {
            var pattern = new GraphThreadPattern()
            {
                DurationSeconds = _config.AgentSettings.FlowDurationSeconds,
                Points = _config.AgentSettings.FlowPoints.ParsePoints()
            };
            _patternService.StartThreadPattern(_config.AgentSettings.FlowPatternExecutionId, _flow, pattern, ct);
        }

        private void StartGradualPattern(CancellationToken ct)
        {
            var pattern = new GradualThreadPattern()
            {
                DurationSeconds = _config.AgentSettings.FlowDurationSeconds,
                Points = _config.AgentSettings.FlowPoints.ParsePoints(),
                RampDownSeconds = _config.AgentSettings.FlowRampDownSeconds,
                RampUpSeconds = _config.AgentSettings.FlowRampUpSeconds,
                StepIntervalSeconds = _config.AgentSettings.FlowStepIntervalSeconds,
                ThreadCount = _config.AgentSettings.FlowThreadCount
            };

            _patternService.StartThreadPattern(_config.AgentSettings.FlowPatternExecutionId, _flow, pattern, ct);
        }
    }
}
