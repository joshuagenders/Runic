using Autofac;
using Microsoft.Extensions.Logging;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.Services.Interfaces;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Standalone.Clients;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Services;
using Runic.Framework.Clients;
using System;
using System.Linq;
using System.Threading;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public class TestEnvironmentBuilder : TestEnvironment
    {
        private ContainerBuilder _builder { get; set; }
        public TestEnvironmentBuilder()
        {
            _builder = new ContainerBuilder();
        }

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
            Register(AgentCoreConfiguration);
            Register(TestResultHandler);
            Register(LoggingHandler);
            Register(AgentObserver);

            return _builder.Build();
        }

        private void Register<T>(TestObject<T> instance) where T : class
        {
            if (!RegisteredTypes.Any(t => t.Item2 == typeof(T)))
            {
                _builder.RegisterInstance(instance.Instance);
            }
        }

        public override TestEnvironment StartApplication()
        {
            var cts = new CancellationTokenSource();
            var container = Build();
            var scope = container.BeginLifetimeScope();
            Application = scope.Resolve<IApplication>();
            return this;
        }

        public TestEnvironmentBuilder WithType<T, U>()
        {
            RegisteredTypes.Add(new Tuple<Type, Type>(typeof(T), typeof(U)));
            _builder.RegisterType<T>().As<U>();
            return this;
        }

        public TestEnvironmentBuilder WithInstance<T>(T obj) where T : class
        {
            _builder.RegisterInstance<T>(obj);
            return this;
        }

        public TestEnvironmentBuilder WithSingleInstanceType<T, U>()
        {
            RegisteredTypes.Add(new Tuple<Type, Type>(typeof(T), typeof(U)));
            _builder.RegisterType<T>().As<U>().SingleInstance();
            return this;
        }

        public TestEnvironmentBuilder WithStandardConfig()
        {
            return WithSingleInstanceType<AgentSettings, IAgentSettings>()
                   .WithSingleInstanceType<StatsdSettings, IStatsdSettings>()
                   .WithSingleInstanceType<AgentConfig, IAgentConfig>()
                   .WithInstance(new AgentCoreConfiguration()
                   {
                       MaxThreads = 10
                   });
        }

        public TestEnvironmentBuilder WithStandardTypes()
        {
            return WithSingleInstanceType<ThreadManager, IThreadManager>()
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
