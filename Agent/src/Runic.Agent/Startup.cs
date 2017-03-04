using Autofac;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.Service;
using StatsN;

namespace Runic.Agent
{
    public class Startup : IStartup
    {
        public IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            IStatsd statsd = Statsd.New<Udp>(options =>
            {
                options.Port = AgentConfiguration.StatsdPort;
                options.HostOrIp = AgentConfiguration.StatsdHost;
                options.Prefix = AgentConfiguration.StatsdPrefix;
                options.BufferMetrics = true;
            });

            builder.RegisterInstance(statsd);
            //builder.RegisterType<StatsdEventHandler>().As<IEventHandler>();
            //builder.RegisterType<NLogEventHandler>().As<IEventHandler>();
            //builder.RegisterType<HttpExecutionService>().As<IExecutionService>();
            builder.RegisterType<FilePluginProvider>().As<IPluginProvider>();
            builder.RegisterType<AgentService>().As<IAgentService>();
            return builder.Build();
        }
    }
}