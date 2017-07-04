using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Metrics;

namespace Runic.Agent.Core.ThreadManagement
{
    public static class FlowExtensions
    {
        public static FlowThreadManager GetFlowThreadManager(
            this IFlowManager flowManager,
            string flow,
            IPluginManager pluginManager, 
            IStats stats, 
            IDataService dataService)
        {
            var flowInstance = flowManager.GetFlow(flow);
            return new FlowThreadManager(
                flowInstance, 
                stats, 
                new FunctionFactory(flowInstance,pluginManager, stats, dataService));
        }
    }
}
