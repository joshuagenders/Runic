using System.Collections.Generic;
using System.Linq;
using Runic.Agent.Configuration;
using Runic.Agent.Harness;

namespace Runic.Agent.Service
{
    public class ExecutionContext
    {
        public Dictionary<string,FlowContext> FlowContexts { get; }
        public Dictionary<string, IFlowHarness> FlowHarnesses { get; }

        public int MaxThreadCount { get; }

        public ExecutionContext()
        {
            FlowContexts = new Dictionary<string, FlowContext>();
            FlowHarnesses = new Dictionary<string, IFlowHarness>();
            MaxThreadCount = AgentConfiguration.MaxThreads;
        }

        public bool ThreadsAreAvailable(int requestedThreadCount, string flow)
        {
            return FlowContexts.Where(f=> f.Key != flow)
                               .Select(f => f.Value.ThreadCount)
                               .Sum() + requestedThreadCount < MaxThreadCount;
        }
    }
}
