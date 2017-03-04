using System.Collections.Generic;
using System.Linq;
using Runic.Agent.Configuration;

namespace Runic.Agent.Service
{
    public class ExecutionContext
    {
        public Dictionary<string,FlowContext> FlowContexts { get; set; }
        public int MaxThreadCount { get; set; }

        public ExecutionContext()
        {
            FlowContexts = new Dictionary<string, FlowContext>();
            MaxThreadCount = AgentConfiguration.MaxThreads;
        }

        public bool ThreadsAreAvailable(int requestedThreadCount, string flow)
        {
            return FlowContexts.Where(f=> f.Key != flow)
                               .Select(f => f.Value.RunningThreadCount)
                               .Sum() + requestedThreadCount < MaxThreadCount;
        }
    }
}
