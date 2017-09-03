using System;
using Autofac;
using Moq;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Providers;
using Runic.Agent.TestUtility;
using System.Threading;
using Runic.Agent.Core.Configuration;
using Runic.Framework.Clients;
using Runic.Agent.Core.ExternalInterfaces;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public class StandaloneTestEnvironment : TestEnvironmentBuilder, IDisposable
    {
        private IApplication _application { get; set; }
        private ILifetimeScope _scope { get; set; }

        public TestObject<IAgentConfig> AgentConfig { get; set; } = new TestObject<IAgentConfig>();
        public TestObject<IAgentSettings> AgentSettings { get; set; } = new TestObject<IAgentSettings>();
        public TestObject<IStatsdSettings> StatsdSettings { get; set; } = new TestObject<IStatsdSettings>();
        public TestObject<IFlowProvider> FlowProvider { get; set; } = new TestObject<IFlowProvider>();
        
        public IApplication Application
        {
            get { return _application ?? new Mock<IApplication>().Object; }
            set { _application = value; }
        }

        public override TestEnvironment StartApplication()
        {
            var cts = new CancellationTokenSource();
            var container = Build();
            var scope = container.BeginLifetimeScope();
            Application = scope.Resolve<IApplication>();
            return this;
        }

        public new StandaloneTestEnvironment WithStandardTypes()
        {
            base.WithStandardTypes();
            WithInstance(FlowProvider.Instance);
            return this;
        }

        public StandaloneTestEnvironment WithStandardConfig()
        {
            WithSingleInstanceType<AgentSettings, IAgentSettings>()
                   .WithSingleInstanceType<StatsdSettings, IStatsdSettings>()
                   .WithSingleInstanceType<AgentConfig, IAgentConfig>()
                   .WithInstance(new AgentCoreConfiguration()
                   {
                       MaxThreads = 10
                   });
            return this;
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
