using Autofac;
using Moq;
using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.ThreadManagement;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using StatsN;
using System;
using System.Linq;
using System.Reflection;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadPatterns;

namespace Runic.Agent.TestUtility
{
    public abstract class TestEnvironment
    {        
        public TestObject<IPluginManager> PluginManager { get; set; } = new TestObject<IPluginManager>();
        public TestObject<IPluginProvider> PluginProvider { get; set; } = new TestObject<IPluginProvider>();
        public TestObject<IFlowManager> FlowManager { get; set; } = new TestObject<IFlowManager>();
        public TestObject<IDataService> DataService { get; set; } = new TestObject<IDataService>();
        public TestObject<IStatsd> Statsd { get; set; } = new TestObject<IStatsd>();
        public TestObject<IStatsClient> StatsClient { get; set; } = new TestObject<IStatsClient>();
        public TestObject<IPatternController> PatternService { get; set; } = new TestObject<IPatternController>();
        public TestObject<IThreadManager> ThreadManager { get; set; } = new TestObject<IThreadManager>();
        public TestObject<IRuneClient> RuneClient { get; set; } = new TestObject<IRuneClient>();
        public TestObject<IDatetimeService> DatetimeService { get; set; } = new TestObject<IDatetimeService>();
        public TestObject<IRunnerService> RunnerService { get; set; } = new TestObject<IRunnerService>();
        public TestObject<IFunctionFactory> FunctionFactory { get; set; } = new TestObject<IFunctionFactory>();
        public TestObject<AgentCoreConfiguration> AgentCoreConfiguration { get; set; } = new TestObject<AgentCoreConfiguration>();
        public TestObject<ITestResultHandler> TestResultHandler { get; set; } = new TestObject<ITestResultHandler>();
        public TestObject<ILoggingHandler> LoggingHandler { get; set; } = new TestObject<ILoggingHandler>();
        public TestObject<TestContext> TestContext { get; set; } = new TestObject<TestContext>();

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
    }
}
