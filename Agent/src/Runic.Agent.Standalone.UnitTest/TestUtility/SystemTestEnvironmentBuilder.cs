using Microsoft.Extensions.Logging;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Standalone.Clients;
using Runic.Agent.Standalone.Logging;
using Runic.Agent.Standalone.Services;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using System.IO;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public class SystemTestEnvironmentBuilder
    {
        public IRuneClient RuneClient { get; set; }
        public IPluginProvider PluginProvider { get; set; }
        public ITestResultHandler TestResultHandler { get; set; }
        public ILoggingHandler LoggingHandler { get; set; }
        public IAgentObserver AgentObserver { get; set; }
        public IDataService DataService { get; set; }

        public TestEnvironmentBuilder New()
        {
            var loggerFactory = new LoggerFactory().AddConsole();

            var builder = new TestEnvironmentBuilder().WithStandardTypes();
            builder.RuneClient.Instance = RuneClient ?? new InMemoryRuneClient();
            builder.DataService.Instance = DataService ?? new NoOpDataService();
            //builder.PluginProvider.Instance = PluginProvider ?? new FilePluginProvider(Directory.GetCurrentDirectory(), "Plugins");
            //builder.TestResultHandler.Instance = TestResultHandler ?? new TestResultHandler();
            builder.LoggingHandler.Instance = LoggingHandler ?? new LoggingHandler(loggerFactory);
            builder.AgentObserver.Instance = AgentObserver ?? new AgentObserver();
            builder.TestContext.Instance = new TestContext()
            {
                DeploymentDirectory = Directory.GetCurrentDirectory(),
                FlowExecutionId = builder.AgentSettings.Instance?.FlowPatternExecutionId,
                ThreadPatternName = builder.AgentSettings.Instance?.FlowThreadPatternName,
                PluginDirectory = "Plugins"
            };

            return builder;
        }
    }
}
