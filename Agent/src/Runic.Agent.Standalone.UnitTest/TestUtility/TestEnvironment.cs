using Autofac;
using Microsoft.Extensions.Logging;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.Services.Interfaces;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Standalone.Clients;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Providers;
using Runic.Agent.Standalone.Services;
using Runic.Framework.Clients;
using StatsN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public class TestEnvironment : IDisposable
    {
        private IApplication _application { get; set; }
        private ILifetimeScope _scope { get; set; }
        private List<Tuple<Type,Type>> _registeredTypes { get; set; }
        private ContainerBuilder _builder { get; set; }

        public IApplication Application
        {
            get
            {
                return _application ?? new Mock<IApplication>().Object;
            }
            set
            {
                _application = value;
            }
        }

        public TestEnvironment()
        {
            _builder = new ContainerBuilder();
            _registeredTypes = new List<Tuple<Type, Type>>();
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
        
        public IContainer Build()
        {

            Register(PluginManager);
            Register(PluginProvider);
            Register(FlowManager);
            Register(DataService);
            Register(Statsd);
            Register(StatsClient);
            Register(PatternService);
            Register(ThreadManager);
            Register(RuneClient);
            Register(AgentConfig);
            Register(AgentSettings);
            Register(StatsdSettings);
            Register(FlowProvider);
            Register(DatetimeService);
            Register(RunnerService);
            Register(FunctionFactory);

            return _builder.Build();
        }

        private void Register<T>(TestObject<T> instance) where T : class
        {
            if (!_registeredTypes.Any(t => t.Item2 == typeof(T)))
            {
                _builder.RegisterInstance(instance.Instance);
            }
        }

        public TestEnvironment WithType<T, U>()
        {
            _registeredTypes.Add(new Tuple<Type, Type>(typeof(T), typeof(U)));
            _builder.RegisterType<T>().As<U>();
            return this;
        }

        public TestEnvironment WithInstance<T>(T obj) where T : class
        {
            _builder.RegisterInstance<T>(obj);
            return this;
        }

        public TestEnvironment WithSingleInstanceType<T, U>()
        {
            _registeredTypes.Add(new Tuple<Type, Type>(typeof(T), typeof(U)));
            _builder.RegisterType<T>().As<U>().SingleInstance();
            return this;
        }

        public TestEnvironment StartApplication()
        {
            var cts = new CancellationTokenSource();
            var container = Build();
            var scope = container.BeginLifetimeScope();
            Application = scope.Resolve<IApplication>();
            return this;
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }

    public static class TestEnvironmentExtensions
    {
        public static TestEnvironment With<T>(this TestEnvironment env, T instance) where T : class
        {
            TestObject<T> testObjectInstance = (TestObject<T>)Activator.CreateInstance(typeof(TestObject<T>));
            testObjectInstance.Instance = instance;
            var prop = env.GetType().GetTypeInfo()
                                    .GetProperties()
                                    .Where(p => p.PropertyType == typeof(TestObject<T>));
            if (!prop.Any())
                throw new ArgumentException($"Test object for {typeof(T).Name} type was not found");

            prop.First().SetValue(env, instance);
            return env;
        }

        public static T Get<T>(this TestEnvironment env) where T : TestObject<T>
        {
            var prop = env.GetType().GetTypeInfo()
                                    .GetProperties()
                                    .Where(p => p.PropertyType == typeof(TestObject<T>));
            if (!prop.Any())
                throw new ArgumentException($"Test object for {typeof(T).Name} type was not found");

            var propValue = (TestObject<T>)prop.First().GetValue(env);
            return propValue.Instance;
        }

        public static TestEnvironment WithStandardConfig(this TestEnvironment env)
        {
            return env.WithSingleInstanceType<AgentSettings, IAgentSettings>()
                      .WithSingleInstanceType<StatsdSettings, IStatsdSettings>()
                      .WithSingleInstanceType<AgentConfig, IAgentConfig>()
                      .WithInstance(new AgentCoreConfiguration()
                      {
                          MaxThreads = 10
                      });
        }

        public static TestEnvironment WithStandardTypes(this TestEnvironment env)
        {
            return env.WithSingleInstanceType<ThreadManager, IThreadManager>()
                      .WithSingleInstanceType<InMemoryRuneClient, IRuneClient>()
                      .WithSingleInstanceType<FlowManager, IFlowManager>()
                      .WithSingleInstanceType<PatternService, IPatternService>()
                      .WithType<NoOpDataService, IDataService>()
                      .WithType<LoggerFactory, ILoggerFactory>()
                      .WithType<StatsClient, IStatsClient>()
                      .WithType<RunnerService, IRunnerService>()
                      .WithType<FunctionFactory, IFunctionFactory>()
                      .WithSingleInstanceType<Application, IApplication>();
        }
    }
}
