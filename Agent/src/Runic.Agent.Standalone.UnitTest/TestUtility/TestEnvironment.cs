using Autofac;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Standalone.Clients;
using Runic.Agent.Standalone.Configuration;
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
            }
        }
    }

    public class TestEnvironment : IDisposable
    {
        private IApplication _application { get; set; }
        private ILifetimeScope _scope { get; set; }

        public List<Tuple<Type, Type>> RegisteredTypes { get; set; }
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
            RegisteredTypes = new List<Tuple<Type, Type>>();
            _builder = new ContainerBuilder();
        }

        public TestObject<IPluginManager> PluginManager { get; set; }
        public TestObject<IPluginProvider> PluginProvider { get; set; }
        public TestObject<IFlowManager> FlowManager { get; set; }
        public TestObject<IDataService> DataService { get; set; }
        public TestObject<IStatsd> Statsd { get; set; }
        public TestObject<IStatsClient> StatsClient{ get; set; }
        public TestObject<IPatternService> PatternService { get; set; }
        public TestObject<IThreadManager> ThreadManager { get; set; }
        public TestObject<IRuneClient> RuneClient { get; set; }
        public TestObject<IAgentConfig> AgentConfig { get; set; }
        public TestObject<IAgentSettings> AgentSettings { get; set; }
        public TestObject<IStatsdSettings> StatsdSettings { get; set; }

        public IContainer Build()
        {
            RegisteredTypes.ForEach(t => _builder.RegisterType(t.Item1).As(t.Item2));

            Register(PluginManager);
            Register(PluginProvider);
            Register(FlowManager.Instance);
            Register(DataService.Instance);
            Register(Statsd);
            Register(StatsClient);
            Register(PatternService.Instance);
            Register(ThreadManager.Instance);
            Register(RuneClient.Instance);
            Register(AgentConfig.Instance);
            Register(AgentSettings.Instance);
            Register(StatsdSettings.Instance);

            return _builder.Build();
        }

        private void Register<T>(T instance) where T : class
        {
            var implementedInterfaces = RegisteredTypes
                                               .Where(s => 
                                                    instance.GetType()
                                                            .GetTypeInfo()
                                                            .ImplementedInterfaces
                                                            .Any(i => i.IsAssignableFrom(s.Item2)))
                                                            .ToList();
            if (!implementedInterfaces.Any())
            {
                _builder.RegisterInstance(instance);
            }
        }

        public TestEnvironment WithType<T, U>()
        {
            _builder.RegisterType<T>().As<U>();
            return this;
        }

        public TestEnvironment WithSingleInstanceType<T, U>()
        {
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
            return env.WithType<Statsd, IStatsd>()
                      .WithSingleInstanceType<ThreadManager, IThreadManager>()
                      .WithSingleInstanceType<InMemoryRuneClient, IRuneClient>()
                      .WithSingleInstanceType<PatternService, IPatternService>()
                      .WithType<Application, IApplication>();
        }
    }
}
