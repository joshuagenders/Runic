using System.Collections.Concurrent;
using Runic.Core.Models;

namespace Runic.Agent.FlowManagement
{
    public class Flows
    {
        //todo replace with a proper management system
        private static ConcurrentDictionary<string, Flow> _flows = new ConcurrentDictionary<string, Flow>();

        public static void AddUpdateFlow(Flow flow)
        {
            _flows.AddOrUpdate(flow.Name, flow, (key, val) => flow);
        }

        public static Flow GetFlow(string name)
        {
            return _flows[name];
        }
    }
}
