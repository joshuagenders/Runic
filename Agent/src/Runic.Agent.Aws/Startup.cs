using Autofac;
using Microsoft.Extensions.Logging;
using Runic.Agent.Aws.Clients;
using Runic.Agent.Aws.Configuration;
using Runic.Agent.Aws.Providers;
using Runic.Agent.Aws.Services;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.IoC;
using Runic.Agent.Standalone.Logging;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using StatsN;
using System.IO;

namespace Runic.Agent.Aws
{
    public class Startup : IStartup
    {
        public IContainer BuildContainer(string[] args = null)
        {
            var builder = DefaultContainer.GetDefaultContainerBuilder();
            builder.RegisterType<S3DataService>().As<IDataService>().SingleInstance();
            builder.RegisterType<SnsRuneClient>().As<IRuneClient>().SingleInstance();
            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<StatsClient>().As<IStatsClient>().SingleInstance();
            builder.RegisterType<AgentConfig>().As<IAgentConfig>().SingleInstance();
            builder.RegisterType<AgentSettings>().As<IAgentSettings>().SingleInstance();
            builder.RegisterType<StatsdSettings>().As<IStatsdSettings>().SingleInstance();
            builder.RegisterType<LoggingHandler>().As<ILoggingHandler>();
            builder.RegisterType<S3FlowProvider>().As<IFlowProvider>();
            
            //todo nxlog
            var loggerFactory = new LoggerFactory();
            builder.RegisterInstance(loggerFactory).SingleInstance();

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

            return builder.Build();
        }
    }   
}
