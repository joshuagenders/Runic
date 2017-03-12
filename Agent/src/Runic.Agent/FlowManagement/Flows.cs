using System.Collections.Concurrent;
using Autofac;
using Runic.Framework.Models;
using StatsN;

namespace Runic.Agent.FlowManagement
{
    public static class Flows
    {
        //todo replace with a proper management system
        private static ConcurrentDictionary<string, Flow> _flows = new ConcurrentDictionary<string, Flow>();
        
        public static void AddUpdateFlow(Flow flow)
        {
            _flows[flow.Name] = flow;
            IoC.Container?.Resolve<IStatsd>()?.Count($"{flow.Name}.AddOrUpdated");
        }

        public static Flow GetFlow(string name)
        {
            IoC.Container?.Resolve<IStatsd>()?.Count($"{name}.get");
            return _flows[name];
        }
    }
}
