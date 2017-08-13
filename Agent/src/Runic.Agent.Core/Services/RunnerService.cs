using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.CucumberHarness;
using Runic.Agent.Core.Exceptions;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services.Interfaces;
using Runic.Framework.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Services
{
    public class RunnerService : IRunnerService
    {
        private readonly ILoggingHandler _log;
        private readonly IFunctionFactory _functionFactory;
        private readonly IDatetimeService _datetimeService;
        private readonly IPluginManager _pluginManager;
        private readonly ITestResultHandler _testResultHandler;

        private int _errorCount { get; set; }
        private int _maxErrors { get; set; } = -1;

        public int ErrorCount => _errorCount;

        public RunnerService(IPluginManager pluginManager, IFunctionFactory functionFactory, IDatetimeService datetimeService, ITestResultHandler testResultHandler, ILoggingHandler loggingHandler)
        {
            _functionFactory = functionFactory;
            _datetimeService = datetimeService;
            _pluginManager = pluginManager;
            _testResultHandler = testResultHandler;
            _log = loggingHandler;
        }

        public async Task ExecuteFlowAsync(Flow flow, CancellationToken ctx = default(CancellationToken))
        {
            Result result = null;
            IStepRunnerService service;
            IStepController stepController;
            if (flow.Steps.All(s => s.Distribution != null))
            {
                stepController = new DistributionStepController(flow.Steps);
            }
            else
            {
                stepController = new StandardStepController(flow);
            }
            _testResultHandler.OnFlowStart(flow);
            while (!ctx.IsCancellationRequested)
            {
                //todo testcontext
                var testContext = new TestContext();
                var step = stepController.GetNextStep(result);
                if (!string.IsNullOrWhiteSpace(step.Cucumber?.Document))
                {
                    service = new CucumberStepRunnerService(_pluginManager);
                    var executionResult = await service.ExecuteStepAsync(step, ctx);
                    _testResultHandler.OnCucumberComplete(executionResult as CucumberResult);
                    result = executionResult;
                }
                else
                {
                    service = new FunctionStepRunnerService(_functionFactory, _datetimeService);
                    var executionResult = await service.ExecuteStepAsync(step, ctx);
                    _testResultHandler.OnFunctionComplete(executionResult as FunctionResult);
                    result = executionResult;
                }
                LogResult(result);
                
                await _datetimeService.WaitUntil(flow.StepDelayMilliseconds, ctx);
            }
            _testResultHandler.OnFlowComplete(flow);
        }

        private void LogResult(Result result)
        {
            if (result.Success)
            {
                _log.Info("Success", result);
            }
            else
            {
                _log.Error("Error", result);
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
