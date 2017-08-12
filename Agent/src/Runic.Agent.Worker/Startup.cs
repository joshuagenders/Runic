using Autofac;
using Microsoft.Extensions.Logging;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.IoC;
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

            var builder = DefaultContainer.GetDefaultContainerBuilder()
                .Register<MessagingDataService, IDataService>()
                .Register<RabbitMessagingService, IMessagingService>()
                .Register<InMemoryRuneClient, IRuneClient>()
                .Register<HandlerRegistry, IHandlerRegistry>()
                .Register<Application, IApplication>();

            var loggerFactory = new LoggerFactory().AddConsole();
            builder.RegisterInstance(loggerFactory).SingleInstance();

            builder.RegisterInstance(statsd).As<IStatsd>();
            builder.RegisterType<FilePluginProvider>()
                    .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                    .As<IPluginProvider>();
        
            return builder.Build();
        }
    }   
}
