using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Metrics;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Worker.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Worker.UnitTest.TestUtility
{
    public class TestEnvironment : IApplication
    {
        public IPluginManager PluginManager { get; set; }
        public IStats Stats { get; set; }
        public IDataService DataService { get; set; }
        public IMessagingService MessagingService { get; set; }
        public IPatternService PatternService { get; set; }
        public IThreadManager ThreadManager { get; set; }
        public IFlowManager FlowManager { get; set; }

        public TestEnvironment(
            IPluginManager pluginManager,
            IStats stats,
            IDataService dataService,
            IMessagingService messagingService,
            IPatternService patternService,
            IThreadManager threadManager,
            IFlowManager flowManager)
        {
            PluginManager = PluginManager;
            Stats = stats;
            DataService = dataService;
            MessagingService = messagingService;
            PatternService = patternService;
            ThreadManager = threadManager;
            FlowManager = flowManager;
        }

        public Task RunApplicationAsync(CancellationToken ct = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
