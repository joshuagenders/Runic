using Runic.Agent.AssemblyManagement;
using Runic.Agent.Data;
using Runic.Agent.FlowManagement;
using Runic.Agent.Services;
using Runic.Agent.Metrics;
using Runic.Agent.ThreadManagement;
using System.Threading;
using System.Threading.Tasks;
using System;
using Runic.Agent.Messaging;

namespace Runic.Agent.UnitTest.TestUtility
{
    public class FakeApplication : IApplication
    {
        private const string wd = "C:\\code\\Runic\\Agent\\src\\Runic.Agent.UnitTest\\bin\\Debug\\netcoreapp1.0";
        public IPluginManager PluginManager { get; set; }
        public IMessagingService MessagingService { get; set; }
        public IFlowManager FlowManager { get; set; }
        public IStats Stats { get; set; }
        public IDataService DataService { get; set; }
        public IThreadOrchestrator ThreadOrchestrator { get; set; }
        public GradualFlowService GradualFlowService { get; set; }
        public ConstantFlowService ConstantFlowService { get; set; }
        public GraphFlowService GraphFlowService { get; set; }

        public FakeApplication(
            IPluginManager pluginManager,
            IFlowManager flowManager,
            IStats stats,
            IDataService dataService,
            IMessagingService messagingService,
            IThreadOrchestrator threadOrchestrator,
            IHandlerRegistry handlerRegistry
            )
        {
            PluginManager = pluginManager;
            FlowManager = flowManager;
            Stats = stats;
            DataService = dataService;
            MessagingService = messagingService;
            ThreadOrchestrator = threadOrchestrator;
            
            GradualFlowService = new GradualFlowService(ThreadOrchestrator);
            ConstantFlowService = new ConstantFlowService(ThreadOrchestrator);
            GraphFlowService = new GraphFlowService(ThreadOrchestrator);
            handlerRegistry.RegisterMessageHandlers();
        }

        public async Task RunApplicationAsync(CancellationToken ct = default(CancellationToken))
        {
            await MessagingService.RunServiceAsync(ct);
        }
    }
}
