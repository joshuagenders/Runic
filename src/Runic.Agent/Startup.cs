using Autofac;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;

namespace Runic.Agent
{
    public class Startup : IStartup
    {
        public void ConfigureApplication()
        {
            //Metrics.ConfigureAsync(new MetricsConfig
            //{
            //    StatsdServerName = AgentConfiguration.Config["Statsd:ServerName"],
            //    Prefix = AgentConfiguration.Config["Statsd:Prefix"]
            //});
        }

        public IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            //builder.RegisterType<StatsdEventHandler>().As<IEventHandler>();
            //builder.RegisterType<NLogEventHandler>().As<IEventHandler>();
            //builder.RegisterType<HttpExecutionService>().As<IExecutionService>();
            builder.RegisterType<FilePluginProvider>().As<IPluginProvider>();
            return builder.Build();
        }
    }
}
