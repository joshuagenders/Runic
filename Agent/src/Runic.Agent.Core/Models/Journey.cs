using System.Collections.Generic;

namespace Runic.Agent.Core.Models
{
    public class Journey
    {
        public string Name { get; set; }
        public int StepDelayMilliseconds { get; set; }
        public List<Step> Steps { get; set; }
        public string AssemblyName { get; set; }
    }
}