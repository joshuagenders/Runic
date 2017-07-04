using Autofac;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Metrics;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Standalone.Clients;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Services;
using Runic.Framework.Clients;
using StatsN;
using System.IO;

namespace Runic.Agent.Standalone
{
    public class Startup : IStartup
    {
        public IContainer BuildContainer(string[] args = null)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<PluginManager>().As<IPluginManager>();
            builder.RegisterType<FlowManager>().As<IFlowManager>().SingleInstance();
            builder.RegisterType<NoOpDataService>().As<IDataService>().SingleInstance();
            builder.RegisterType<InMemoryRuneClient>().As<IRuneClient>().SingleInstance();
            builder.RegisterType<PatternService>().As<IPatternService>().SingleInstance();
            builder.RegisterType<ThreadManager>().As<IThreadManager>().SingleInstance();
            builder.RegisterType<Application>().As <IApplication>();
            builder.RegisterType<Stats>().As<IStats>().SingleInstance();

            builder.RegisterType<FilePluginProvider>()
                   .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                   .As<IPluginProvider>();

            

            IStatsd statsd = Statsd.New<Udp>(options =>
            {
                options.Port = AgentConfig.StatsdSettings.StatsdPort.Value;
                options.HostOrIp = AgentConfig.StatsdSettings.StatsdHost;
                options.Prefix = AgentConfig.StatsdSettings.StatsdPrefix;
                options.BufferMetrics = false;
            });
            builder.RegisterInstance(statsd).As<IStatsd>();
            
            return builder.Build();
        }
    }   
}
