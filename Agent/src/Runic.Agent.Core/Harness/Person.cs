using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Harness
{
    public class Person : IPerson
    {
        private readonly IFunctionFactory _functionFactory;
        private readonly IDatetimeService _datetimeService;
        private readonly IAssemblyManager _assemblyManager;
        private Dictionary<string, string> _attributes { get; set; }

        public Person(IFunctionFactory functionFactory, IDatetimeService datetimeService, IAssemblyManager assemblyManager)
        {
            _functionFactory = functionFactory;
            _datetimeService = datetimeService;
            _assemblyManager = assemblyManager;
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
                    _assemblyManager.LoadAssembly(step.Cucumber.AssemblyName);
                    var assembly = _assemblyManager.GetAssembly(step.Cucumber.AssemblyName);
                    service = new CucumberStepRunnerService(assembly);
                    var executionResult = await service.ExecuteStepAsync(step, ctx);
                    result = executionResult;
                }
                else
                {
                    _assemblyManager.LoadAssembly(step.Function.AssemblyName);
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
