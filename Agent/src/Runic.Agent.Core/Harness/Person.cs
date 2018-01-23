using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Models;
using System.Collections.Generic;
using System.Threading;

namespace Runic.Agent.Core.Harness
{
    public class Person
    {
        private readonly IAssemblyManager _assemblyManager;

        public Person(IAssemblyManager assemblyManager)
        {
            _assemblyManager = assemblyManager;
        }

        public List<Result> PerformJourney(Journey journey)
        {
            var results = new List<Result>();
            foreach(var step in journey.Steps)
            {
                Result result = null;
                if (!string.IsNullOrWhiteSpace(step.Cucumber?.Document))
                {
                    _assemblyManager.LoadAssembly(step.Cucumber.AssemblyName);
                    var assembly = _assemblyManager.GetAssembly(step.Cucumber.AssemblyName);
                    result = new CucumberHarness().ExecuteTest(assembly, step.Cucumber.Document);
                }
                else
                {
                    _assemblyManager.LoadAssembly(step.Function.AssemblyName);
                    var assembly = _assemblyManager.GetAssembly(step.Cucumber.AssemblyName);
                    result = new FunctionHarness().ExecuteTest(assembly, step);
                }
                results.Add(result);
                Thread.Sleep(journey.StepDelayMilliseconds);
            }
            return results;
        }
    }
}
