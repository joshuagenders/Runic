using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Framework.Models;

namespace Runic.Agent.Core.FlowManagement
{
    public static class JourneyInitialiser
    {
        public static void InitialiseJourney(IAssemblyManager assemblyManager, Journey journey)
        {
            foreach (var step in journey.Steps)
            {
                if (step.Function != null)
                {
                    assemblyManager.LoadAssembly(step.Function.AssemblyName);
                }
                else if (step.Cucumber != null)
                {
                    assemblyManager.LoadAssembly(step.Cucumber.AssemblyName);
                }
            }
        }
    }
}
