using System.IO;
using Autofac;
using RawRabbit.DependencyInjection.Autofac;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.Harness;
using Runic.Agent.Messaging;
using Runic.Agent.Service;
using StatsN;
using Runic.Framework.Clients;
using Runic.Agent.Metrics;

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
            builder.RegisterRawRabbit();
            builder.RegisterType<RabbitMessageClient>().As<IRuneClient>();

            return builder.Build();
        }
    }
}