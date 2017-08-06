using Microsoft.Extensions.Logging;
using Runic.Agent.Core.Harness;
using Runic.Framework.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadManagement
{
    public class FlowRunner
    {
        private readonly FunctionFactory _factory;
        private readonly CucumberHarness _harness;
        private readonly Flow _flow;
        private readonly ILogger _logger;
        private int _maxErrors { get; set; } = -1;
        private bool _getNextStepFromResult { get; set; }
        public int ErrorCount { get; set; }

        public FlowRunner(FunctionFactory factory, CucumberHarness harness, Flow flow, ILoggerFactory loggerFactory, int maxErrors)
        {
            _factory = factory;
            _harness = harness;
            _flow = flow;
            _logger = loggerFactory.CreateLogger<FlowRunner>();
            _maxErrors = maxErrors;
        }

        private async Task ExecuteFunctionAsync(CancellationToken ctx = default(CancellationToken))
        {
            FunctionHarness function = null;
            FunctionResult result = null;
            while (!ctx.IsCancellationRequested)
            {
                Step step = null;
                //todo
                var testContext = new TestContext()
                {
                };

                
                if (_getNextStepFromResult && result?.NextStep != null)
                {
                    var matchingStep = _flow.Steps
                                            .Where(s => s.StepName == result.StepName)
                                            .Select(s => s);
                    if (!matchingStep.Any())
                    {
                        throw new StepNotFoundException($"Step not found for step {result.StepName}");
                    }
                    if (matchingStep.Count() > 1)
                    {
                        throw new StepNotFoundException($"Duplicate step found for step {result.StepName}");
                    }
                    step = matchingStep.Single();
                }

                if (function == null)
                {
                    step = _flow.Steps.First();
                    _getNextStepFromResult = step.GetNextStepFromFunctionResult;
                }
                else
                {
                    var matchingStep = _flow.Steps
                                           .Where(s => s.StepName == result.StepName)
                                           .Select(s => _flow.Steps.IndexOf(s));
                    int functionIndex = matchingStep.Single();

                    //handle null for stepname in result
                    functionIndex++;
                    functionIndex = functionIndex >= _flow.Steps.Count ? 0 : functionIndex;
                    step = _flow.Steps[functionIndex];
                }
                if (step == null)
                    throw new StepNotFoundException($"Step not found for step {result.StepName}");

                function = _factory.CreateFunction(step, testContext);
                _getNextStepFromResult = step.GetNextStepFromFunctionResult;
                result = await function.OrchestrateFunctionExecutionAsync(ctx);
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
                ErrorCount++;
                if (_maxErrors >= 0 && ErrorCount >= _maxErrors)
                {
                    throw new AggregateException(
                        result.Exception, 
                        new MaxErrorCountExceededException($"Error count reached {ErrorCount}"));
                }
            }
        }

        private async Task ExecuteCucumberAsync(CancellationToken ctx = default(CancellationToken))
        {
            while (!ctx.IsCancellationRequested)
            {
                foreach (var step in _flow.Steps)
                {
                    if (step.Cucumber != null)
                    {
                        var result = await _harness.ExecuteTestAsync(step.Cucumber.AssemblyName, step.Cucumber.Document, ctx);
                        LogResult(result);
                    }
                }
            }
        }

        public async Task ExecuteFlowAsync(CancellationToken ctx = default(CancellationToken))
        {
            if (_flow.Steps.Any(s => !string.IsNullOrEmpty(s.Cucumber?.Document)))
            {
                //todo allow for mix of functions and cucumber execution in a flow
                await ExecuteCucumberAsync(ctx);
            }
            else
            {
                await ExecuteFunctionAsync(ctx);
            }
        }
    }
}
