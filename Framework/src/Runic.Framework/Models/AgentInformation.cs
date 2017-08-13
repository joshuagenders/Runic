using System;
using System.Collections.Generic;

namespace Runic.Framework.Models
{
    public class AgentInformation
    {
        public List<PatternInformation> Patterns { get; set; }
        public List<FlowInformation> Flows { get; set; }
        public int MaxThreadCount { get; set; }
    }

    public class PatternInformation
    {
        public string PatternExecutionId { get; set; }
        public string PatternType { get; set; }
    }

    public class FlowInformation
    {
        public string FlowExecutionId { get; set; }
        public int ThreadCount { get; set; }
    }
}
