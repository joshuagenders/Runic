using System.Collections.Immutable;

namespace Runic.Agent.Core.Models
{
    public class Journey
    {
        public Journey(
            string name, 
            int stepDelayMilliseconds, 
            ImmutableList<Step> steps)
        {
            Name = name;
            StepDelayMilliseconds = stepDelayMilliseconds;
            Steps = steps;
        }

        public string Name { get; }
        public int StepDelayMilliseconds { get; }
        public ImmutableList<Step> Steps { get; }
    }
}