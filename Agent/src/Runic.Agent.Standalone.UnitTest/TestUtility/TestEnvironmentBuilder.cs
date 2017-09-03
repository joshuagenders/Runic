using Autofac;
using Microsoft.Extensions.Logging;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Agent.Standalone.Clients;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Services;
using Runic.Framework.Clients;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public class TestEnvironmentBuilder : TestEnvironment
    {
        private ContainerBuilder _builder { get; set; }
        public TestEnvironmentBuilder()
        {
            _builder = new ContainerBuilder();
            RegisterTestObjects();
        }

        public IContainer Build()
        {
            return _builder.Build();
        }

        public TestEnvironmentBuilder With<T>(T instance) where T : class
        {
            var props = GetType().GetTypeInfo()
                                 .GetProperties();

            var prop = props.Where(p => p.PropertyType == typeof(TestObject<T>) &&
                                        p.PropertyType.GenericTypeArguments[0] == typeof(T));
            if (prop.Any())
            {
                var testObj = prop.First().GetValue(this);
                testObj.GetType().GetProperty("Instance").SetValue(testObj, instance);
            }

            _builder.RegisterInstance(instance);
            return this;
        }

        protected virtual void RegisterTestObjects()
        {
            var props = GetType().GetTypeInfo()
                                 .GetProperties();
            var genericProps = props.Where(p => p.PropertyType.IsConstructedGenericType);
            var testObjects = genericProps.Where(p => p.PropertyType.GetGenericTypeDefinition().Equals(typeof(TestObject<>)))
                                          .ToList();

            foreach (var prop in testObjects)
            {
                var testObject = prop.GetValue(this);
                var instance = testObject.GetType().GetProperty("Instance").GetValue(testObject);
                var asType = prop.PropertyType.GenericTypeArguments[0];
                _builder.RegisterInstance(instance).As(asType);
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
                   .WithSingleInstanceType<PatternController, IPatternController>()
                   .WithType<NoOpDataService, IDataService>()
                   .WithType<LoggerFactory, ILoggerFactory>()
                   .WithType<StatsClient, IStatsClient>()
                   .WithType<RunnerService, IRunnerService>()
                   .WithType<FunctionFactory, IFunctionFactory>()
                   .WithSingleInstanceType<Application, IApplication>();
        }
    }
}
