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

namespace Runic.Agent
{
    public class Startup : IStartup
    {
        public IContainer Register()
        {
            var builder = new ContainerBuilder();

            IStatsd statsd = Statsd.New<Udp>(options =>
            {
                options.Port = AgentConfiguration.Instance.StatsdPort;
                options.HostOrIp = AgentConfiguration.Instance.StatsdHost;
                options.Prefix = AgentConfiguration.Instance.StatsdPrefix;
                options.BufferMetrics = false;
            });
            Stats.Statsd = statsd;

            builder.RegisterInstance(new Flows());
            builder.RegisterInstance(new PluginManager());

            builder.RegisterType<AgentService>().As<IAgentService>();
            builder.RegisterType<NoOpMessagingService>().As<IMessagingService>();
            builder.RegisterType<InMemoryClient>().As<IRuneClient>();
            builder.RegisterType<FilePluginProvider>()
                    .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                    .As<IPluginProvider>();

            //builder.RegisterRawRabbit();
            //builder.RegisterType<RabbitMessageClient>().As<IRuneClient>();
            //builder.RegisterType<RabbitMessagingService>().As<IMessagingService>();

            return builder.Build();
        }
    }
}