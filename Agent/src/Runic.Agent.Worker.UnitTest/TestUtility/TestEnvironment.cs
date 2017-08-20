using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Worker.Messaging;
using Runic.Framework.Clients;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Worker.Test.TestUtility
{
    public class TestEnvironment : IApplication
    {
        public IDatetimeService DatetimeService { get; set; }
        public IRunnerService RunnerService { get; set; }
        public IPluginManager PluginManager { get; set; }
        public IStatsClient Stats { get; set; }
        public IDataService DataService { get; set; }
        public IMessagingService MessagingService { get; set; }
        public IPatternService PatternService { get; set; }
        public IThreadManager ThreadManager { get; set; }
        public IFlowManager FlowManager { get; set; }
        public IHandlerRegistry HandlerRegistry { get; set; }

        public TestEnvironment(
            IPluginManager pluginManager,
            IStatsClient stats,
            IDataService dataService,
            IMessagingService messagingService,
            IPatternService patternService,
            IThreadManager threadManager,
            IFlowManager flowManager,
            IHandlerRegistry handlerRegistry,
            IDatetimeService datetimeService,
            IRunnerService runnerService
            )
        {
            PluginManager = pluginManager;
            Stats = stats;
            DataService = dataService;
            MessagingService = messagingService;
            PatternService = patternService;
            ThreadManager = threadManager;
            FlowManager = flowManager;
            HandlerRegistry = handlerRegistry;
            DatetimeService = datetimeService;
            RunnerService = runnerService;
        }

        public Task RunApplicationAsync(CancellationToken ctx = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
