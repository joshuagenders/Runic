using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Metrics;
using Runic.Agent.Core.ThreadManagement;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.TestUtility
{
    public class FakeApplication : IApplication
    {
        private const string wd = "C:\\code\\Runic\\Agent\\src\\Runic.Agent.Core.UnitTest\\bin\\Debug\\netcoreapp1.0";
        public IPluginManager PluginManager { get; set; }
        public IFlowManager FlowManager { get; set; }
        public IStats Stats { get; set; }
        public IDataService DataService { get; set; }
        public IPatternService ThreadOrchestrator { get; set; }
        
        public FakeApplication(
            IPluginManager pluginManager,
            IFlowManager flowManager,
            IStats stats,
            IDataService dataService,
            IPatternService threadOrchestrator
            )
        {
            PluginManager = pluginManager;
            FlowManager = flowManager;
            Stats = stats;
            DataService = dataService;
            ThreadOrchestrator = threadOrchestrator;            
        }

        public async Task RunApplicationAsync(CancellationToken ct = default(CancellationToken))
        {
            await Task.FromResult(true);
        }
    }
}
