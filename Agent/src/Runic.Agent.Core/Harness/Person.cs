using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Harness
{
    public class Person
    {
        private readonly AssemblyService _assemblyService;

        public Person(AssemblyService assemblyService)
        {
            _assemblyService = assemblyService;
        }
        
        public async Task<List<Result>> PerformJourneyAsync(Journey journey)
        {
            var results = new List<Result>();
            foreach (var step in journey.Steps)
            {
                Result result = null;
                if (!string.IsNullOrWhiteSpace(step.Cucumber?.Document))
                {
                    var assembly = _assemblyService.GetLoadAssembly(step.Cucumber.AssemblyPath);
                    result = await new CucumberHarness().ExecuteTestAsync(assembly, step);
                }
                else
                {
                    var assembly = _assemblyService.GetLoadAssembly(step.Function.AssemblyPath);
                    result = await new FunctionHarness().ExecuteTestAsync(assembly, step);
                }
                results.Add(result);
                await Task.Delay(journey.StepDelayMilliseconds);
            }
            return results;
        }
    }
}
