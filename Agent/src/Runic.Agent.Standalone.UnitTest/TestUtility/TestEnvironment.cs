using Autofac;
using Microsoft.Extensions.Logging;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Metrics;
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
    public class TestObject<T> where T : class
    {
        private Mock<T> _mockObject { get; set; }
        public Mock<T> MockObject
        {
            get
            {
                if (_mockObject == null)
                    _mockObject = new Mock<T>();
                return _mockObject;
            }
            private set { _mockObject = value; }
        }
        public bool HasInstance { get; set; } = false;
        private T _instance;
        public T Instance
        {
            get
            {
                return _instance ?? MockObject.Object;
            }
            set
            {
                _instance = value;
                HasInstance = true;
            }
        }
    }

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
        public static TestEnvironment WithPluginManager(this TestEnvironment env, IPluginManager instance)
        {
            env.PluginManager.Instance = instance;
            return env;
        }

        public static TestEnvironment WithPluginProvider(this TestEnvironment env, IPluginProvider instance)
        {
            env.PluginProvider.Instance = instance;
            return env;
        }

        public static TestEnvironment WithDataService(this TestEnvironment env, IDataService instance)
        {
            env.DataService.Instance = instance;
            return env;
        }

        public static TestEnvironment WithStatsd(this TestEnvironment env, IStatsd instance)
        {
            env.Statsd.Instance = instance;
            return env;
        }

        public static TestEnvironment WithPatternService(this TestEnvironment env, IPatternService instance)
        {
            env.PatternService.Instance = instance;
            return env;
        }

        public static TestEnvironment WithStatsClient(this TestEnvironment env, IStatsClient instance)
        {
            env.StatsClient.Instance = instance;
            return env;
        }

        public static TestEnvironment WithThreadManager(this TestEnvironment env, IThreadManager instance)
        {
            env.ThreadManager.Instance = instance;
            return env;
        }

        public static TestEnvironment WithRuneClient(this TestEnvironment env, IRuneClient instance)
        {
            env.RuneClient.Instance = instance;
            return env;
        }

        public static TestEnvironment WithAgentSettings(this TestEnvironment env, IAgentSettings instance)
        {
            env.AgentSettings.Instance = instance;
            return env;
        }

        public static TestEnvironment WithAgentConfig(this TestEnvironment env, IAgentConfig instance)
        {
            env.AgentConfig.Instance = instance;
            return env;
        }
        
        public static TestEnvironment WithFlowManager(this TestEnvironment env, IFlowManager instance)
        {
            env.FlowManager.Instance = instance;
            return env;
        }

        public static TestEnvironment WithStatsdSettings(this TestEnvironment env, IStatsdSettings instance)
        {
            env.StatsdSettings.Instance = instance;
            return env;
        }

        public static TestEnvironment WithStandardConfig(this TestEnvironment env)
        {
            return env.WithSingleInstanceType<AgentSettings, IAgentSettings>()
                      .WithSingleInstanceType<StatsdSettings, IStatsdSettings>()
                      .WithSingleInstanceType<AgentConfig, IAgentConfig>();
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
                      .WithSingleInstanceType<Application, IApplication>();
        }
    }
}
