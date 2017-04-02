using Runic.Agent.AssemblyManagement;
using Runic.Agent.Data;
using Runic.Agent.FlowManagement;
using Runic.Agent.Metrics;

namespace Runic.Agent.Harness
{
    public class ThreadManagerFactory : IThreadManagerFactory
    {
        private readonly IPluginManager _pluginManager;
        private readonly IStats _stats;
        private readonly IDataService _dataService;
        private readonly IFlowManager _flowManager;

        public ThreadManagerFactory(IFlowManager flowManager, IPluginManager pluginManager, IStats stats, IDataService dataService)
        {
            _flowManager = flowManager;
            _pluginManager = pluginManager;
            _stats = stats;
            _dataService = dataService;
        }

        public ThreadManager Get(string flow)
        {
            return new ThreadManager(_flowManager.GetFlow(flow), _pluginManager, _stats, _dataService);
        }
    }
}
