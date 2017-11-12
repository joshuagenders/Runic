using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Framework.Models;

namespace Runic.Agent.Core.FlowManagement
{
    public class JourneyInitialiser
    {
        private readonly IAssemblyManager _assemblyManager;
        
        public JourneyInitialiser(IAssemblyManager assemblyManager)
        {
            _assemblyManager = assemblyManager;
        }

        public void InitialiseJourney(Journey journey)
        {
            foreach (var step in journey.Steps)
            {
                if (step.Function != null)
                {
                    _assemblyManager.LoadAssembly(step.Function.AssemblyName);
                }
                else if (step.Cucumber != null)
                {
                    _assemblyManager.LoadAssembly(step.Cucumber.AssemblyName);
                }
            }
        }
    }
}
