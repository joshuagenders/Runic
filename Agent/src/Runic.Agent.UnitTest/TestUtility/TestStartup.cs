using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using StatsN;
using Runic.Agent.Metrics;
using Runic.Agent.Configuration;
using Runic.Agent.FlowManagement;
using Runic.Agent.Messaging;
using Runic.Framework.Clients;
using Runic.Agent.AssemblyManagement;
using System.IO;
using Runic.Agent.Service;
using Runic.Agent.Data;

namespace Runic.Agent.UnitTest.TestUtility
{
    public class TestStartup : IStartup
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
            builder.RegisterType<DataService>().As<IDataService>();
            builder.RegisterType<NoOpMessagingService>().As<IMessagingService>();
            builder.RegisterType<InMemoryClient>().As<IRuneClient>();
            builder.RegisterType<FilePluginProvider>()
                    .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                    .As<IPluginProvider>();
            builder.RegisterType<PluginManager>().As<IPluginManager>();

            builder.RegisterType<AgentService>().As<IAgentService>();

            return builder.Build();
        }
    }
}
