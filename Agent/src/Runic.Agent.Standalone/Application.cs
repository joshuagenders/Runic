using Microsoft.Extensions.Logging;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone
{
    public class Application : IApplication
    {
        private readonly IPatternService _patternService;
        private readonly IFlowManager _flowManager;
        private readonly ILogger _logger;
        private readonly IAgentConfig _config;

        public Application(IPatternService patternService, IFlowManager flowManager, ILoggerFactory loggerFactory, IAgentConfig config)
        {
            _patternService = patternService;
            _flowManager = flowManager;
            _logger = loggerFactory.CreateLogger<IApplication>();
        }

        public async Task RunApplicationAsync(CancellationToken ct = default(CancellationToken))
        {
            _logger.LogInformation("Run application invoked");
            var executionService = new ConfigExecutionService(_patternService, _flowManager, _config);
            await executionService.StartThreadPattern(ct);
            await _patternService.GetCompletionTaskAsync(_config.AgentSettings.FlowPatternExecutionId, ct);
        }
    }
}
