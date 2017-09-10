using Autofac;
using Microsoft.Extensions.Logging;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.IoC;
using Runic.Agent.Core.Services;
using Runic.Agent.Standalone.Clients;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Logging;
using Runic.Agent.Standalone.Providers;
using Runic.Agent.Standalone.Services;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using StatsN;
using System.Collections.Generic;
using System.IO;

namespace Runic.Agent.Standalone
{
    public class Startup : IStartup
    {
        public IContainer BuildContainer(string[] args = null)
        {
            var builder = new ContainerBuilder();
            TypeList.GetDefaultTypeList().ForEach(t => builder.RegisterType(t.Item1).As(t.Item2));
            builder.RegisterType<DataService>().As<IDataService>().SingleInstance();
            builder.RegisterType<InMemoryRuneClient>().As<IRuneClient>().SingleInstance();
            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<AgentConfig>().As<IAgentConfig>().SingleInstance();
            builder.RegisterType<AgentSettings>().As<IAgentSettings>().SingleInstance();
            builder.RegisterType<FunctionFactory>().As<IFunctionFactory>().SingleInstance();
            builder.RegisterType<FileTestDataService>().As<ITestDataService>();
            builder.RegisterType<StatsdSettings>().As<IStatsdSettings>().SingleInstance();
            builder.RegisterType<FileFlowProvider>().As<IFlowProvider>();
            builder.RegisterType<FilePluginProvider>()
                   .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                   .WithParameter(new PositionalParameter(1, "Plugins"))
                   .As<IPluginProvider>();
            
            RegisterInstances(builder);

            return builder.Build();
        }

        protected virtual void RegisterInstances(ContainerBuilder builder)
        {
            var statsdSettings = new StatsdSettings();
            var agentSettings = new AgentSettings();
            var agentConfig = new AgentConfig(statsdSettings, agentSettings);

            builder.RegisterInstance(new TestContext()
            {
                DeploymentDirectory = Directory.GetCurrentDirectory(),
                FlowExecutionId = agentConfig.AgentSettings.FlowPatternExecutionId,
                ThreadPatternName = agentConfig.AgentSettings.FlowThreadPatternName,
                PluginDirectory = "Plugins"
            });

            IStatsd statsd = Statsd.New<Udp>(options =>
            {
                options.Port = agentConfig.StatsdSettings.Port;
                options.HostOrIp = agentConfig.StatsdSettings.Host;
                options.Prefix = agentConfig.StatsdSettings.Prefix;
                options.BufferMetrics = false;
            });

            builder.RegisterInstance(statsd).As<IStatsd>().SingleInstance();
            var statsHandler = new StatsHandler(statsd);
            var loggerFactory = new LoggerFactory();
            var loggingHandler = new LoggingHandler(loggerFactory);

            builder.RegisterType<IEventHandler>()
                   .WithParameter(new PositionalParameter(0, new List<IEventHandler>() { statsHandler, loggingHandler }));
        }
    }   
}
