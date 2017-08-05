using Newtonsoft.Json;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Providers;
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
        private IFlowProvider _flowProvider;

        public ConfigExecutionService(IPatternService patternService, IFlowManager flowManager, IAgentConfig config, IFlowProvider flowProvider)
        {
            _patternService = patternService;
            _flowManager = flowManager;
            _config = config;
            _flowProvider = flowProvider;
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
            _flowManager.AddUpdateFlow(_flow);
        }

        private void StartConstantPattern(CancellationToken ctx = default(CancellationToken))
        {
            var pattern = new ConstantThreadPattern()
                {
                    ThreadCount = _config.AgentSettings.FlowThreadCount,
                    DurationSeconds = _config.AgentSettings.FlowDurationSeconds
                };

            _patternService.StartThreadPattern(_config.AgentSettings.FlowPatternExecutionId, _flow, pattern, ctx);
        }

        private void StartGraphPattern(CancellationToken ctx = default(CancellationToken))
        {
            var pattern = new GraphThreadPattern()
            {
                DurationSeconds = _config.AgentSettings.FlowDurationSeconds,
                Points = _config.AgentSettings.FlowPoints.ParsePoints()
            };
            _patternService.StartThreadPattern(_config.AgentSettings.FlowPatternExecutionId, _flow, pattern, ctx);
        }

        private void StartGradualPattern(CancellationToken ctx = default(CancellationToken))
        {
            var pattern = new GradualThreadPattern()
            {
                DurationSeconds = _config.AgentSettings.FlowDurationSeconds,
                RampDownSeconds = _config.AgentSettings.FlowRampDownSeconds,
                RampUpSeconds = _config.AgentSettings.FlowRampUpSeconds,
                StepIntervalSeconds = _config.AgentSettings.FlowStepIntervalSeconds,
                ThreadCount = _config.AgentSettings.FlowThreadCount
            };

            _patternService.StartThreadPattern(_config.AgentSettings.FlowPatternExecutionId, _flow, pattern, ctx);
        }
    }
}
