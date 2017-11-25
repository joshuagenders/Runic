using Runic.Agent.Core.Models;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.StepController;
using Runic.Agent.Harness;
using System.Reflection;
using System.Collections.Generic;

namespace Runic.Agent.Services
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
            Result result = null;
            IStepRunnerService service;
            var stepController = new StandardStepController(journey);

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

                if (ctx.IsCancellationRequested)
                    break;
                await _datetimeService.WaitMilliseconds(journey.StepDelayMilliseconds, ctx);
            }
        }
    }
}
