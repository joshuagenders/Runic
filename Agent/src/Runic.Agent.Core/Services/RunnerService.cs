using Microsoft.Extensions.Logging;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Exceptions;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services.Interfaces;
using Runic.Framework.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Services
{
    public class RunnerService : IRunnerService
    {
        private readonly ILogger<RunnerService> _logger;
        private readonly IFunctionFactory _functionFactory;
        private readonly IDatetimeService _datetimeService;
        private readonly IPluginManager _pluginManager;
        private int _errorCount { get; set; }
        private int _maxErrors { get; set; } = -1;

        public int ErrorCount => _errorCount;

        public RunnerService(IPluginManager pluginManager, ILoggerFactory loggerFactory, IFunctionFactory functionFactory, IDatetimeService datetimeService)
        {
            _logger = loggerFactory.CreateLogger<RunnerService>();
            _functionFactory = functionFactory;
            _datetimeService = datetimeService;
            _pluginManager = pluginManager;
        }

        public async Task ExecuteFlowAsync(Flow flow, CancellationToken ctx = default(CancellationToken))
        {
            Result result = null;
            IStepRunnerService service;
            var stepController = new StepController(flow);
            while (!ctx.IsCancellationRequested)
            {
                //todo testcontext
                var testContext = new TestContext();
                var step = stepController.GetNextStep(result);

                if (!string.IsNullOrWhiteSpace(step.Cucumber?.Document))
                {
                    service = new FunctionStepRunnerService(_functionFactory, _datetimeService);
                }
                else
                {
                    service = new CucumberStepRunnerService(_pluginManager);
                }

                result = await service.ExecuteStepAsync(step, ctx);
                await _datetimeService.WaitUntil(flow.StepDelayMilliseconds, ctx);
                LogResult(result);
            }
        }

        private void LogResult(Result result)
        {
            if (result.Success)
            {
                _logger.LogTrace("Success", result);
            }
            else
            {
                _logger.LogError("Error", result);
                _errorCount++;
                if (_maxErrors >= 0 && ErrorCount >= _maxErrors)
                {
                    throw new AggregateException(
                        result.Exception,
                        new MaxErrorCountExceededException($"Error count reached {ErrorCount}"));
                }
            }
        }
    }
}
