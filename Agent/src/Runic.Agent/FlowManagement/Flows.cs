using System.Collections.Concurrent;
using Autofac;
using Runic.Core.Models;
using StatsN;

namespace Runic.Agent.FlowManagement
{
    public class Flows
    {
        //todo replace with a proper management system
        private static ConcurrentDictionary<string, Flow> _flows = new ConcurrentDictionary<string, Flow>();
        private static IStatsd _statsd = IoC.Container?.Resolve<IStatsd>();

        public static void AddUpdateFlow(Flow flow)
        {
            _flows.AddOrUpdate(flow.Name, flow, (key, val) => flow);
            _statsd.Count("{flow.Name}.AddOrUpdated");
        }

        public static Flow GetFlow(string name)
        {
            _statsd.Count("{flow.Name}.get");
            return _flows[name];
        }
    }
}
