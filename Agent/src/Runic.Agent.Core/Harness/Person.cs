using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Harness
{
    public class Person
    {
        private readonly IAssemblyManager _assemblyManager;

        public Person(IAssemblyManager assemblyManager)
        {
            _assemblyManager = assemblyManager;
        }
        
        public async Task<List<Result>> PerformJourneyAsync(Journey journey)
        {
            var results = new List<Result>();
            foreach (var step in journey.Steps)
            {
                Result result = null;
                if (!string.IsNullOrWhiteSpace(step.Cucumber?.Document))
                {
                    _assemblyManager.LoadAssembly(step.Cucumber.AssemblyName);
                    var assembly = _assemblyManager.GetAssembly(step.Cucumber.AssemblyName);
                    result = await new CucumberHarness().ExecuteTestAsync(assembly, step.Cucumber.Document);
                }
                else
                {
                    _assemblyManager.LoadAssembly(step.Function.AssemblyName);
                    var assembly = _assemblyManager.GetAssembly(step.Function.AssemblyName);
                    result = await new FunctionHarness().ExecuteTestAsync(assembly, step);
                }
                results.Add(result);
                await Task.Delay(journey.StepDelayMilliseconds);
            }
            return results;
        }
    }
}
