using Autofac;
using Microsoft.Extensions.Logging;
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
using System.IO;

namespace Runic.Agent.Standalone
{
    public class Startup : IStartup
    {
        public IContainer BuildContainer(string[] args = null)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<PluginManager>().As<IPluginManager>().SingleInstance();
            builder.RegisterType<FlowManager>().As<IFlowManager>().SingleInstance();
            builder.RegisterType<NoOpDataService>().As<IDataService>().SingleInstance();
            builder.RegisterType<InMemoryRuneClient>().As<IRuneClient>().SingleInstance();
            builder.RegisterType<PatternService>().As<IPatternService>().SingleInstance();
            builder.RegisterType<ThreadManager>().As<IThreadManager>().SingleInstance();
            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<StatsClient>().As<IStatsClient>().SingleInstance();
            builder.RegisterType<AgentConfig>().As<IAgentConfig>().SingleInstance();
            builder.RegisterType<AgentSettings>().As<IAgentSettings>().SingleInstance();
            builder.RegisterType<StatsdSettings>().As<IStatsdSettings>().SingleInstance();
            builder.RegisterType<FileFlowProvider>().As<IFlowProvider>();
            builder.RegisterType<FilePluginProvider>()
                   .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                   .WithParameter(new PositionalParameter(1, "Plugins"))
                   .As<IPluginProvider>();

            var loggerFactory = new LoggerFactory().AddConsole();
            builder.RegisterInstance(loggerFactory).SingleInstance();

            var statsdSettings = new StatsdSettings();
            var agentSettings = new AgentSettings();
            var agentConfig = new AgentConfig(statsdSettings, agentSettings);

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
