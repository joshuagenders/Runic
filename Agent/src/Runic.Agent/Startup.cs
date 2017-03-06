using System.IO;
using Autofac;
using RawRabbit.DependencyInjection.Autofac;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.Harness;
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
            builder.RegisterType<FilePluginProvider>()
                    .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                    .As<IPluginProvider>();
            builder.RegisterType<AgentService>().As<IAgentService>();
            builder.RegisterType<RabbitMessagingService>().As<IMessagingService>();
            builder.RegisterType<FlowHarness>().As<IFlowHarness>();
            builder.RegisterType<FunctionHarness>().As<IFunctionHarness>();

            builder.RegisterRawRabbit(AgentConfiguration.ClientConnectionConfiguration);

            return builder.Build();
        }
    }
}