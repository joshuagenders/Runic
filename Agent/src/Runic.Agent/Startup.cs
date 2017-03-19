using System.IO;
using Autofac;
using RawRabbit.DependencyInjection.Autofac;
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
        public IContainer RegisterDependencies()
        {            
            IStatsd statsd = Statsd.New<Udp>(options =>
            {
                options.Port = AgentConfiguration.Instance.StatsdPort;
                options.HostOrIp = AgentConfiguration.Instance.StatsdHost;
                options.Prefix = AgentConfiguration.Instance.StatsdPrefix;
                options.BufferMetrics = true;
            });
            Clients.Statsd = statsd;

            return BuildContainer();
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<FilePluginProvider>()
                    .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                    .As<IPluginProvider>();
            builder.RegisterType<AgentService>().As<IAgentService>();
            builder.RegisterInstance(new Flows());
            //builder.RegisterRawRabbit();
            //builder.RegisterType<RabbitMessageClient>().As<IRuneClient>();
            //builder.RegisterType<RabbitMessagingService>().As<IMessagingService>();
            builder.RegisterType<NoOpMessagingService>().As<IMessagingService>();
            builder.RegisterType<NoOpRuneClient>().As<IRuneClient>();
            return builder.Build();
        }
    }
}