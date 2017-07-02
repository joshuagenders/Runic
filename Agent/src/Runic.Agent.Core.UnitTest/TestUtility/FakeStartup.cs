﻿//using Autofac;
//using Runic.Agent.Core.AssemblyManagement;
//using Runic.Agent.Core.Configuration;
//using Runic.Agent.Core.FlowManagement;
//using Runic.Agent.Core.Metrics;
//using Runic.Agent.Core.ThreadManagement;
//using Runic.Framework.Clients;
//using StatsN;
//using System.IO;
//
//namespace Runic.Agent.Core.UnitTest.TestUtility
//{
//    public class FakeStartup
//    {
//        public IContainer BuildContainer(string [] args)
//        {
//            AgentConfiguration.LoadConfiguration(args);
//
//            IStatsd statsd = Statsd.New<Udp>(options =>
//            {
//                options.Port = AgentConfiguration.Instance.StatsdPort;
//                options.HostOrIp = AgentConfiguration.Instance.StatsdHost;
//                options.Prefix = AgentConfiguration.Instance.StatsdPrefix;
//                options.BufferMetrics = false;
//            });
//
//            var builder = new ContainerBuilder();
//
//            builder.RegisterInstance(statsd).As<IStatsd>();
//            builder.RegisterType<Stats>().As<IStats>();
//            builder.RegisterType<FlowManager>().As<IFlowManager>();
//            //builder.RegisterInstance(new Mock<IMessagingService>().Object).As<IMessagingService>();
//            builder.RegisterType<InMemoryClient>().As<IRuneClient>();
//            builder.RegisterType<FilePluginProvider>()
//                    .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
//                    .As<IPluginProvider>();
//            builder.RegisterType<PluginManager>().As<IPluginManager>();
//            builder.RegisterType<PatternService>().As<IPatternService>();
//            //builder.RegisterType<HandlerRegistry>().As<IHandlerRegistry>();
//            //builder.RegisterInstance(new InMemoryMessagingService()).As<IMessagingService>();
//
//            return builder.Build();
//        }
//    }
//}
