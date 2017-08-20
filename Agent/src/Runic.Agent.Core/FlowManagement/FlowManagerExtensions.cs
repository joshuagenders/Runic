using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.ThreadManagement;
using Runic.Framework.Clients;
using Runic.Agent.Core.Services;

namespace Runic.Agent.Core.FlowManagement
{
    public static class FlowManagerExtensions
    {
        public static FlowThreadManager GetFlowThreadManager(
            this IFlowManager flowManager,
            string flow,
            IPluginManager pluginManager,
            IStatsClient stats,
            IRunnerService runnerService,
            ILoggingHandler loggingHandler,
            AgentCoreConfiguration config)
        {
            var flowInstance = flowManager.GetFlow(flow);
            return new FlowThreadManager(flowInstance, stats, runnerService, loggingHandler);
        }
    }
}
