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

        public ConfigExecutionService(IPatternService patternService, IFlowManager flowManager)
        {
            _patternService = patternService;
            _flowManager = flowManager;
        }

        public async Task StartThreadPattern(CancellationToken ct = default(CancellationToken))
        {
            //todo standardise tokens
            ImportFlow();
            switch (AgentConfig.AgentSettings.ThreadPatternName.Value.ToLowerInvariant())
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
            if (!File.Exists(AgentConfig.AgentSettings.FlowFilepath))
            {
                throw new FileNotFoundException($"Flow not found at {AgentConfig.AgentSettings.FlowFilepath}");
            }
            _flow = JsonConvert.DeserializeObject<Flow>(File.ReadAllText(AgentConfig.AgentSettings.FlowFilepath));
            _flowManager.AddUpdateFlow(_flow);
        }

        private void StartConstantPattern(CancellationToken ct = default(CancellationToken))
        {
            var pattern = new ConstantThreadPattern()
                {
                    ThreadCount = AgentConfig.AgentSettings.ThreadCount,
                    DurationSeconds = AgentConfig.AgentSettings.DurationSeconds
                };

            _patternService.StartThreadPattern(
                AgentConfig.AgentSettings.PatternExecutionId, 
                _flow, 
                pattern, 
                ct);
        }

        private void StartGraphPattern(CancellationToken ct)
        {
            var pattern = new GraphThreadPattern()
            {
                DurationSeconds = AgentConfig.AgentSettings.DurationSeconds,
                Points = AgentConfig.AgentSettings.Points.Value.ParsePoints()
            };
            _patternService.StartThreadPattern(
                AgentConfig.AgentSettings.PatternExecutionId,
                _flow,
                pattern,
                ct);
        }

        private void StartGradualPattern(CancellationToken ct)
        {
            var pattern = new GradualThreadPattern()
            {
                DurationSeconds = AgentConfig.AgentSettings.DurationSeconds,
                Points = AgentConfig.AgentSettings.Points.Value.ParsePoints(),
                RampDownSeconds = AgentConfig.AgentSettings.RampDownSeconds,
                RampUpSeconds = AgentConfig.AgentSettings.RampUpSeconds,
                StepIntervalSeconds = AgentConfig.AgentSettings.StepIntervalSeconds,
                ThreadCount = AgentConfig.AgentSettings.ThreadCount
            };

            _patternService.StartThreadPattern(
               AgentConfig.AgentSettings.PatternExecutionId,
               _flow,
               pattern,
               ct);
        }
    }
}
