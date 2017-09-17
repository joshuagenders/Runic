using Moq;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Providers;
using Microsoft.Extensions.Logging;
using Runic.Agent.Standalone.Logging;
using Runic.Agent.TestHarness.Services;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public sealed class StandaloneTestEnvironment : TestEnvironmentBuilder
    {
        private IApplication _application { get; set; }
        
        public TestObject<IAgentConfig> AgentConfig { get; set; } = new TestObject<IAgentConfig>();
        public TestObject<IAgentSettings> AgentSettings { get; set; } = new TestObject<IAgentSettings>();
        public TestObject<IStatsdSettings> StatsdSettings { get; set; } = new TestObject<IStatsdSettings>();
        public TestObject<IFlowProvider> FlowProvider { get; set; } = new TestObject<IFlowProvider>();
        public TestObject<IDatetimeService> DateTimeService { get; set; } = new TestObject<IDatetimeService>();
        public Task ApplicationTask { get; private set; }
        public IApplication Application
        {
            get { return _application ?? new Mock<IApplication>().Object; }
            set { _application = value; }
        }

        public override ITestEnvironment StartApplication()
        {
            With(AgentConfig);
            With(AgentSettings);
            With(StatsdSettings);
            With(FlowProvider);
            With(DatetimeService);
            var loggerFactory = new LoggerFactory();
            With<ILoggerFactory>(loggerFactory);

            var loggingHandler = new LoggingHandler(loggerFactory);
            With(loggingHandler);

            var serviceCollection = Build();
            _application = serviceCollection.GetService(typeof(IApplication)) as IApplication;
            ApplicationTask = _application.RunApplicationAsync();
            return this;
        }
    }
}
