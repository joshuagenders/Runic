using System;
using Autofac;
using Runic.Agent.Worker.Configuration;
using Moq;
using StatsN;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Worker.Services;
using Runic.Agent.Worker.Clients;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Worker.Messaging;
using Runic.Framework.Clients;
using System.IO;
using Runic.Agent.Core.Metrics;

namespace Runic.Agent.Worker.UnitTest.TestUtility
{
    public class Startup : IStartup
    {
        public IContainer BuildContainer(string[] args = null)
        {
            WorkerConfiguration.LoadConfiguration(args);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(new Mock<IDataService>().Object).As<IDataService>();
            builder.RegisterInstance(new Mock<IStats>().Object).As<IStats>();

            IStatsd statsd = new Mock<IStatsd>().Object;
            builder.RegisterInstance(statsd).As<IStatsd>();

            builder.Register<FlowManager, IFlowManager>()
                   .Register<PluginManager, IPluginManager>()
                   .Register<ThreadManager, IThreadManager>()
                   .Register<InMemoryRuneClient, IRuneClient>()
                   .Register<PatternService, IPatternService>()
                   .Register<HandlerRegistry, IHandlerRegistry>()
                   .Register<InMemoryMessagingService, IMessagingService>()
                   .Register<MessagingDataService, IDataService>()
                   .Register<TestEnvironment, IApplication>();
            
            builder.RegisterType<FilePluginProvider>()
                    .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                    .As<IPluginProvider>();

            return builder.Build();
        }
    }
}
