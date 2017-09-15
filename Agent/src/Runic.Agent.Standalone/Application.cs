using Microsoft.Extensions.Logging;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Providers;
using Runic.Agent.Standalone.Services;
using Runic.Agent.TestHarness.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone
{
    public class Application : IApplication
    {
        private readonly IFlowPatternController _patternController;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IAgentConfig _config;
        private readonly IFlowProvider _flowProvider;
        private readonly IDatetimeService _datetimeService;

        public Application(IFlowPatternController patternController, ILoggerFactory loggerFactory, IAgentConfig config, IFlowProvider flowProvider, IDatetimeService datetimeService)
        {
            _patternController = patternController;
            _logger = loggerFactory.CreateLogger<IApplication>();
            _loggerFactory = loggerFactory;
            _config = config;
            _flowProvider = flowProvider;
            _datetimeService = datetimeService;
        }

        public async Task RunApplicationAsync(CancellationToken ctx = default(CancellationToken))
        {
            _logger.LogInformation("Creating execution service.");
            var executionService = new ConfigExecutionService(_patternController, _config, _flowProvider, _loggerFactory, _datetimeService);
            await executionService.StartThreadPattern(ctx);
            _logger.LogInformation("Thread pattern started.");
            await _patternController.Run(ctx);
            _logger.LogInformation("Thread pattern complete.");
        }
    }
}
