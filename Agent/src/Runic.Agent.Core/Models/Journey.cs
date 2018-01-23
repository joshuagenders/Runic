using System.Collections.Generic;

namespace Runic.Agent.Core.Models
{
    public class Journey
    {
        public Journey(
            string name, 
            int stepDelayMilliseconds, 
            IReadOnlyList<Step> steps, 
            string assemblyName)
        {
            Name = name;
            StepDelayMilliseconds = stepDelayMilliseconds;
            Steps = steps;
            AssemblyName = assemblyName;
        }

        public string Name { get; }
        public int StepDelayMilliseconds { get; }
        public IReadOnlyList<Step> Steps { get; }
        public string AssemblyName { get; }
    }
}