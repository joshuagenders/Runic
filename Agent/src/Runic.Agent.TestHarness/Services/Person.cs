using Runic.Agent.Framework.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.TestHarness.StepController;
using Runic.Agent.TestHarness.Harness;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace Runic.Agent.TestHarness.Services
{
    public class Person : IPerson
    {
        private readonly IFunctionFactory _functionFactory;
        private readonly IDatetimeService _datetimeService;
        private readonly Assembly _assembly;
        private Dictionary<string, string> _attributes { get; set; }

        public Person(IFunctionFactory functionFactory, IDatetimeService datetimeService, Assembly assembly)
        {
            _functionFactory = functionFactory;
            _datetimeService = datetimeService;
            _assembly = assembly;
        }

        public void SetAttributes(Dictionary<string, string> attributes)
        {
            _attributes = attributes;
        }

        public async Task PerformJourneyAsync(Journey journey, CancellationToken ctx = default(CancellationToken))
        {
            //TODO pass attributes int journey
            Result result = null;
            IStepRunnerService service;
            IStepController stepController;
            if (journey.Steps.All(s => s.Distribution != null))
            {
                stepController = new DistributionStepController(journey.Steps);
            }
            else
            {
                stepController = new StandardStepController(journey);
            }
            try
            {
                while (!ctx.IsCancellationRequested)
                {
                    var step = stepController.GetNextStep(result);
                    if (!string.IsNullOrWhiteSpace(step.Cucumber?.Document))
                    {
                        service = new CucumberStepRunnerService(_assembly);
                        var executionResult = await service.ExecuteStepAsync(step, ctx);
                        result = executionResult;
                    }
                    else
                    {
                        service = new FunctionStepRunnerService(_functionFactory, _datetimeService);
                        var executionResult = await service.ExecuteStepAsync(step, ctx);
                        result = executionResult;
                    }
                    //_eventService.OnTestResult(result);

                    if (ctx.IsCancellationRequested)
                        break;
                    await _datetimeService.WaitMilliseconds(journey.StepDelayMilliseconds, ctx);
                }
            }
            finally
            {
                //_eventService.OnFlowComplete(flow);
            }
        }
    }
}
