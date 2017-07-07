using Microsoft.Extensions.Logging;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Harness;
using Runic.Framework.Clients;

namespace Runic.Agent.Core.ThreadManagement
{
    public static class FlowExtensions
    {
        public static FlowThreadManager GetFlowThreadManager(
            this IFlowManager flowManager,
            string flow,
            IPluginManager pluginManager,
            IStatsClient stats, 
            IDataService dataService,
            ILoggerFactory loggerFactory)
        {
            var flowInstance = flowManager.GetFlow(flow);
            return new FlowThreadManager(
                flowInstance, 
                stats, 
                new FunctionFactory(flowInstance,pluginManager, stats, dataService, loggerFactory));
        }
    }
}
