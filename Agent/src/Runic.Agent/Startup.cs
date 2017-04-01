using System.IO;
using Autofac;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.Service;
using StatsN;
using Runic.Framework.Clients;
using Runic.Agent.Metrics;
using Runic.Agent.Messaging;
using Runic.Agent.FlowManagement;
using Runic.Agent.Data;

namespace Runic.Agent
{
    public class Startup : IStartup
    {
        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            //builder.RegisterRawRabbit();
            //builder.RegisterType<RabbitMessageClient>().As<IRuneClient>();
            //builder.RegisterType<RabbitMessagingService>().As<IMessagingService>();
            
            IStatsd statsd = Statsd.New<Udp>(options =>
            {
                options.Port = AgentConfiguration.Instance.StatsdPort;
                options.HostOrIp = AgentConfiguration.Instance.StatsdHost;
                options.Prefix = AgentConfiguration.Instance.StatsdPrefix;
                options.BufferMetrics = false;
            });
            builder.RegisterInstance(statsd).As<IStatsd>();
            builder.RegisterType<Stats>().As<IStats>();
            builder.RegisterType<FlowManager>().As<IFlowManager>();
            builder.RegisterType<PluginManager>().As<IPluginManager>();
            builder.RegisterType<DataService>().As<IDataService>();
            builder.RegisterType<NoOpMessagingService>().As<IMessagingService>();
            builder.RegisterType<InMemoryClient>().As<IRuneClient>();
            builder.RegisterType<FilePluginProvider>()
                    .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                    .As<IPluginProvider>();

            builder.RegisterType<AgentService>().As<IAgentService>();

            return builder.Build();
        }
    }
}