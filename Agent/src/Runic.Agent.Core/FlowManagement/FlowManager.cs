using System.Collections.Concurrent;
using Runic.Framework.Models;
using Runic.Agent.Core.Metrics;
using System.Linq;
using System.Collections.Generic;

namespace Runic.Agent.Core.FlowManagement
{
    public class FlowManager : IFlowManager
    {
        private ConcurrentDictionary<string, Flow> _flows { get; set; }
        private readonly IStats _stats;

        public FlowManager(IStats stats)
        {
            _flows = new ConcurrentDictionary<string, Flow>();
            _stats = stats;
        }

        public void AddUpdateFlow(Flow flow)
        {
            _flows[flow.Name] = flow;
            _stats.CountFlowAdded(flow.Name);
        }

        public Flow GetFlow(string name)
        {
            Flow result;
            return _flows.TryGetValue(name, out result) ? _flows[name] : null;
        }

        public IList<Flow> GetFlows()
        {
            return _flows.Values?.ToList();
        }
    }
}
