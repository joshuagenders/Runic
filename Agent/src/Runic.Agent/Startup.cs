using Autofac;
using RawRabbit.DependencyInjection.Autofac;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.Messaging;
using Runic.Agent.Service;
using StatsN;

namespace Runic.Agent
{
    public class Startup : IStartup
    {
        public IContainer RegisterDependencies()
        {            
            IStatsd statsd = Statsd.New<Udp>(options =>
            {
                options.Port = AgentConfiguration.StatsdPort;
                options.HostOrIp = AgentConfiguration.StatsdHost;
                options.Prefix = AgentConfiguration.StatsdPrefix;
                options.BufferMetrics = true;
            });

            var builder = new ContainerBuilder();

            builder.RegisterInstance(statsd);
            builder.RegisterType<FilePluginProvider>().As<IPluginProvider>();
            builder.RegisterType<AgentService>().As<IAgentService>();
            builder.RegisterType<RabbitMessagingService>().As<IMessagingService>();
            builder.RegisterRawRabbit(AgentConfiguration.ClientConnectionConfiguration);

            return builder.Build();
        }
    }
}