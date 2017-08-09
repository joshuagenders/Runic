using Microsoft.Extensions.Logging;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.Services.Interfaces;
using Runic.Agent.Core.ThreadManagement;
using Runic.Framework.Clients;

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
            ILoggerFactory loggerFactory,
            AgentCoreConfiguration config)
        {
            var flowInstance = flowManager.GetFlow(flow);
            return new FlowThreadManager(flowInstance, stats, runnerService, loggerFactory);
        }
    }
}
