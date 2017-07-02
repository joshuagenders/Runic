using Autofac;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Worker.Clients;
using Runic.Agent.Worker.Configuration;
using Runic.Agent.Worker.Messaging;
using Runic.Agent.Worker.Services;
using Runic.Framework.Clients;
using StatsN;
using System.IO;

namespace Runic.Agent.Worker
{
    public class Startup : IStartup
    {
        public IContainer BuildContainer(string[] args = null)
        {
            WorkerConfiguration.LoadConfiguration(args);
            IStatsd statsd = Statsd.New<Udp>(options =>
            {
                options.Port = WorkerConfiguration.Instance.StatsdPort;
                options.HostOrIp = WorkerConfiguration.Instance.StatsdHost;
                options.Prefix = WorkerConfiguration.Instance.StatsdPrefix;
                options.BufferMetrics = false;
            });

            var builder = new ContainerBuilder()
                .Register<FlowManager, IFlowManager>()
                .Register<PluginManager, IPluginManager>()
                .Register<MessagingDataService, IDataService>()
                //.Register<RabbitMessagingService, IDataService>()
                .Register<RabbitMessagingService, IMessagingService>()
                .Register<InMemoryRuneClient, IRuneClient>()
                .Register<PatternService, IPatternService>()
                .Register<HandlerRegistry, IHandlerRegistry>()
                .Register<Application, IApplication>();
            
            builder.RegisterInstance(statsd).As<IStatsd>();
            builder.RegisterType<FilePluginProvider>()
                    .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                    .As<IPluginProvider>();
        
            return builder.Build();
        }
    }   
}
