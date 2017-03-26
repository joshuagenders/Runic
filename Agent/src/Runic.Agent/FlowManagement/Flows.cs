using System.Collections.Concurrent;
using Runic.Framework.Models;
using Runic.Agent.Metrics;
using System.Linq;
using System.Collections.Generic;

namespace Runic.Agent.FlowManagement
{
    public class Flows
    {
        private ConcurrentDictionary<string, Flow> _flows { get; set; }
        
        public Flows()
        {
            _flows = new ConcurrentDictionary<string, Flow>();
        }

        public void AddUpdateFlow(Flow flow)
        {
            _flows[flow.Name] = flow;
            Stats.CountFlowAdded(flow.Name);
        }

        public Flow GetFlow(string name)
        {
            return _flows[name];
        }

        public List<Flow> GetFlows()
        {
            return _flows.Values.ToList();
        }
    }
}
