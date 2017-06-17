using Autofac;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.Data;
using Runic.Agent.FlowManagement;
using Runic.Agent.Messaging;
using Runic.Agent.Metrics;
using Runic.Agent.Services;
using Runic.Agent.ThreadManagement;
using Runic.Framework.Clients;
using StatsN;
using System.IO;

namespace Runic.Agent
{
    public class Startup : IStartup
    {
        public IContainer BuildContainer(string [] args = null)
        {            
            AgentConfiguration.LoadConfiguration(args);
            IStatsd statsd = Statsd.New<Udp>(options =>
            {
                options.Port = AgentConfiguration.Instance.StatsdPort;
                options.HostOrIp = AgentConfiguration.Instance.StatsdHost;
                options.Prefix = AgentConfiguration.Instance.StatsdPrefix;
                options.BufferMetrics = false;
            });

            var builder = new ContainerBuilder();

            builder.RegisterInstance(statsd).As<IStatsd>();
            builder.RegisterType<Stats>().As<IStats>();
            builder.RegisterType<FlowManager>().As<IFlowManager>();
            builder.RegisterType<PluginManager>().As<IPluginManager>();
            builder.RegisterType<JsonDataService>().As<IDataService>();
            builder.RegisterType<NoOpMessagingService>().As<IMessagingService>();
            builder.RegisterType<InMemoryClient>().As<IRuneClient>();
            builder.RegisterType<FilePluginProvider>()
                    .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                    .As<IPluginProvider>();

            builder.RegisterType<ThreadOrchestrator>().As<IThreadOrchestrator>();
            builder.RegisterType<HandlerRegistry>().As<IHandlerRegistry>();

            return builder.Build();
        }
    }
}