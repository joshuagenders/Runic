using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Harness
{
    public class Person : IPerson
    {
        private readonly IFunctionFactory _functionFactory;
        private readonly IDatetimeService _datetimeService;
        private readonly IAssemblyManager _assemblyManager;

        public Person(IFunctionFactory functionFactory, IDatetimeService datetimeService, IAssemblyManager assemblyManager)
        {
            _functionFactory = functionFactory;
            _datetimeService = datetimeService;
            _assemblyManager = assemblyManager;
        }

        private async Task<CucumberResult> ExecuteCucumber(Assembly assembly, Step step, CancellationToken ctx)
        {
            return await new CucumberHarness().ExecuteTestAsync(assembly, step.Cucumber.Document, ctx);
        }

        private async Task<FunctionResult> ExecuteFunction(Step step, CancellationToken ctx)
        {
            var testContext = new TestContext();
            var function = _functionFactory.CreateFunction(step, testContext);
            return await function.ExecuteAsync(ctx);
        }

        public async Task PerformJourneyAsync(Journey journey, CancellationToken ctx = default(CancellationToken))
        {
            Result result = null;
            var stepController = new StandardStepController(journey);

            while (!ctx.IsCancellationRequested)
            {
                var step = stepController.GetNextStep(result);
                if (!string.IsNullOrWhiteSpace(step.Cucumber?.Document))
                {
                    _assemblyManager.LoadAssembly(step.Cucumber.AssemblyName);
                    var assembly = _assemblyManager.GetAssembly(step.Cucumber.AssemblyName);
                    result = await ExecuteCucumber(assembly, step, ctx);
                }
                else
                {
                    _assemblyManager.LoadAssembly(step.Function.AssemblyName);
                    result = await ExecuteFunction(step, ctx); ;
                }

                if (ctx.IsCancellationRequested)
                    break;
                await _datetimeService.WaitMilliseconds(journey.StepDelayMilliseconds, ctx);
            }
        }
    }
}
