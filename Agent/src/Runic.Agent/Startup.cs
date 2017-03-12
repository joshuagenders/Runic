using System.IO;
using Autofac;
using RawRabbit.DependencyInjection.Autofac;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.Harness;
using Runic.Agent.Messaging;
using Runic.Agent.Service;
using StatsN;
using RawRabbit;
using Runic.Framework.Clients;

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

            var builder = new ContainerBuilder();

            builder.RegisterInstance(statsd);
            builder.RegisterType<FilePluginProvider>()
                    .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                    .As<IPluginProvider>();
            builder.RegisterType<AgentService>().As<IAgentService>();
            builder.RegisterType<FlowHarness>().As<IFlowHarness>();
            builder.RegisterType<FunctionHarness>().As<IFunctionHarness>();
            builder.RegisterRawRabbit();
            builder.RegisterType<BusClient>().As<IBusClient>();
            builder.RegisterType<RabbitMessagingService>().As<IMessagingService>();
            builder.RegisterType<RabbitMessageClient>().As<IRuneClient>();

            return builder.Build();
        }
    }
}