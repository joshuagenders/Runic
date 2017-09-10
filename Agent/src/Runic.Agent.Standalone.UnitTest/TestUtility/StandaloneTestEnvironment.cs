using System;
using Autofac;
using Moq;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Providers;
using Runic.Agent.TestUtility;
using Runic.Agent.Core.Services;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Runic.Framework.Models;
using Runic.Agent.Standalone.Logging;
using Runic.Agent.Core.PluginManagement;
using Runic.Framework.Clients;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public sealed class StandaloneTestEnvironment : Startup
    {
        private IApplication _application { get; set; }
        
        public TestObject<IAgentConfig> AgentConfig { get; set; } = new TestObject<IAgentConfig>();
        public TestObject<IAgentSettings> AgentSettings { get; set; } = new TestObject<IAgentSettings>();
        public TestObject<IStatsdSettings> StatsdSettings { get; set; } = new TestObject<IStatsdSettings>();
        public TestObject<IFlowProvider> FlowProvider { get; set; } = new TestObject<IFlowProvider>();
        public TestObject<IEventHandler> EventHandler { get; set; } = new TestObject<IEventHandler>();
        public TestObject<IDatetimeService> DateTimeService { get; set; } = new TestObject<IDatetimeService>();
        public TestObject<IPluginManager> PluginManager{ get; set; } = new TestObject<IPluginManager>();

        public IApplication Application
        {
            get { return _application ?? new Mock<IApplication>().Object; }
            set { _application = value; }
        }

        public StandaloneTestEnvironment StartApplication()
        {
            var container = BuildContainer();
            var scope = container.BeginLifetimeScope();
            Application = scope.Resolve<IApplication>();
            return this;
        }
   
        protected override void RegisterInstances(ContainerBuilder builder)
        {
            builder.RegisterInstance(new TestContext());
            var loggerFactory = new LoggerFactory();
            builder.RegisterInstance(loggerFactory).As<ILoggerFactory>();
            var loggingHandler = new LoggingHandler(loggerFactory);

            builder.RegisterInstance(AgentConfig.Instance).As<IAgentConfig>();
            builder.RegisterInstance(AgentSettings.Instance).As<IAgentSettings>();
            builder.RegisterInstance(StatsdSettings.Instance).As<IStatsdSettings>();
            builder.RegisterInstance(FlowProvider.Instance).As<IFlowProvider>();
            builder.RegisterInstance(PluginManager.Instance).As<IPluginManager>();

            builder.RegisterType<IEventHandler>()
                   .WithParameter(new PositionalParameter(0, new List<IEventHandler>() { loggingHandler }));
        }
    }

}
