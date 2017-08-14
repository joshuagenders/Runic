using Autofac;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services.Interfaces;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Providers;
using Runic.Framework.Clients;
using StatsN;
using System;
using System.Linq;
using System.Reflection;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public abstract class TestEnvironment : IDisposable
    {
        private IApplication _application { get; set; }
        private ILifetimeScope _scope { get; set; }
        
        public IApplication Application
        {
            get { return _application ?? new Mock<IApplication>().Object; }
            set { _application = value; }
        }

        public TestObject<IPluginManager> PluginManager { get; set; } = new TestObject<IPluginManager>();
        public TestObject<IPluginProvider> PluginProvider { get; set; } = new TestObject<IPluginProvider>();
        public TestObject<IFlowManager> FlowManager { get; set; } = new TestObject<IFlowManager>();
        public TestObject<IDataService> DataService { get; set; } = new TestObject<IDataService>();
        public TestObject<IStatsd> Statsd { get; set; } = new TestObject<IStatsd>();
        public TestObject<IStatsClient> StatsClient{ get; set; } = new TestObject<IStatsClient>();
        public TestObject<IPatternService> PatternService { get; set; } = new TestObject<IPatternService>();
        public TestObject<IThreadManager> ThreadManager { get; set; } = new TestObject<IThreadManager>();
        public TestObject<IRuneClient> RuneClient { get; set; } = new TestObject<IRuneClient>();
        public TestObject<IAgentConfig> AgentConfig { get; set; } = new TestObject<IAgentConfig>();
        public TestObject<IAgentSettings> AgentSettings { get; set; } = new TestObject<IAgentSettings>();
        public TestObject<IStatsdSettings> StatsdSettings { get; set; } = new TestObject<IStatsdSettings>();
        public TestObject<IFlowProvider> FlowProvider { get; set; } = new TestObject<IFlowProvider>();
        public TestObject<IDatetimeService> DatetimeService { get; set; } = new TestObject<IDatetimeService>();
        public TestObject<IRunnerService> RunnerService { get; set; } = new TestObject<IRunnerService>();
        public TestObject<IFunctionFactory> FunctionFactory { get; set; } = new TestObject<IFunctionFactory>();
        public TestObject<AgentCoreConfiguration> AgentCoreConfiguration { get; set; } = new TestObject<AgentCoreConfiguration>();
        public TestObject<ITestResultHandler> TestResultHandler { get; set; } = new TestObject<ITestResultHandler>();
        public TestObject<ILoggingHandler> LoggingHandler { get; set; } = new TestObject<ILoggingHandler>();
        public TestObject<IAgentObserver> AgentObserver { get; set; } = new TestObject<IAgentObserver>();

        public T Get<T>() where T : class
        {
            var prop = GetType().GetTypeInfo()
                                .GetProperties()
                                .Where(p => p.PropertyType == typeof(TestObject<T>));
            if (!prop.Any())
                throw new ArgumentException($"Get<T> Test object for {typeof(T).Name} type was not found");

            var propValue = (TestObject<T>)prop.First().GetValue(this);
            return propValue.Instance;
        }

        public Mock<T> GetMock<T>() where T : class
        {
            var prop = GetType().GetTypeInfo()
                                .GetProperties()
                                .Where(p => p.PropertyType == typeof(TestObject<T>));
            if (!prop.Any())
                throw new ArgumentException($"Test object for {typeof(T).Name} type was not found");

            var propValue = (TestObject<T>)prop.First().GetValue(this);
            return propValue.MockObject;
        }

        public abstract TestEnvironment StartApplication();

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
