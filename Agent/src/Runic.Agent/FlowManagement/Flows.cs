using System.Collections.Concurrent;
using Runic.Core.Models;

namespace Runic.Agent.FlowManagement
{
    public class Flows
    {
        private static ConcurrentDictionary<string, Flow> _flows { get; set; }

        public static void AddUpdateFlow(Flow flow)
        {
            _flows.AddOrUpdate(flow.Name, flow, (key, val) => flow);
        }
    }
}
