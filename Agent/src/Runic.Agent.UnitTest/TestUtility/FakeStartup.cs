using Autofac;
using StatsN;
using Runic.Agent.Metrics;
using Runic.Agent.Configuration;
using Runic.Agent.FlowManagement;
using Runic.Agent.Services;
using Runic.Framework.Clients;
using Runic.Agent.AssemblyManagement;
using System.IO;
using Runic.Agent.Data;
using Runic.Agent.ThreadManagement;
using Moq;
using Runic.Agent.Messaging;

namespace Runic.Agent.UnitTest.TestUtility
{
    public class FakeStartup : IStartup
    {
        public IContainer BuildContainer(string [] args)
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
            builder.RegisterType<JsonDataService>().As<IDataService>();
            builder.RegisterInstance(new Mock<IMessagingService>().Object).As<IMessagingService>();
            builder.RegisterType<InMemoryClient>().As<IRuneClient>();
            builder.RegisterType<FilePluginProvider>()
                    .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                    .As<IPluginProvider>();
            builder.RegisterType<PluginManager>().As<IPluginManager>();
            builder.RegisterType<ThreadOrchestrator>().As<IThreadOrchestrator>();
            builder.RegisterType<FakeApplication>().As<IApplication>();

            builder.RegisterType<HandlerRegistry>().As<IHandlerRegistry>();
            builder.RegisterInstance(new InMemoryMessagingService()).As<IMessagingService>();

            return builder.Build();
        }
    }
}
