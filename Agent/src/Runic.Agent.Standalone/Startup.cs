using Microsoft.Extensions.Logging;
using Runic.Agent.Framework.ExternalInterfaces;
using Runic.Agent.TestHarness.Harness;
using Runic.Agent.Core.IoC;
using Runic.Agent.Core.Services;
using Runic.Agent.Standalone.Clients;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Logging;
using Runic.Agent.Standalone.Providers;
using Runic.Agent.Standalone.Services;
using Runic.Agent.Framework.Clients;
using Runic.Agent.Framework.Models;
using StatsN;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.Configuration;

namespace Runic.Agent.Standalone
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IRuneClient, InMemoryRuneClient>();
            serviceCollection.AddTransient<IApplication, Application>();
            serviceCollection.AddSingleton<IAgentConfig, AgentConfig>();
            serviceCollection.AddTransient<IAgentSettings, AgentSettings>();
            serviceCollection.AddTransient<ITestDataService, FileTestDataService>();
            serviceCollection.AddTransient<IStatsdSettings, StatsdSettings>();
            serviceCollection.AddTransient<IFlowProvider, FileFlowProvider>();
            serviceCollection.AddTransient<ITestDataService, FileTestDataService>();
            
            CoreServiceCollection.ConfigureServices(serviceCollection);
            AddSingletons(serviceCollection);
        }

        private static void AddSingletons(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPluginProvider>(new FilePluginProvider(Directory.GetCurrentDirectory(), "Plugins"));

            var statsdSettings = new StatsdSettings();
            var agentSettings = new AgentSettings();
            var agentConfig = new AgentConfig(statsdSettings, agentSettings);

            serviceCollection.AddSingleton(
                new TestContext()
                {
                    DeploymentDirectory = Directory.GetCurrentDirectory(),
                    FlowExecutionId = agentConfig.AgentSettings.FlowPatternExecutionId,
                    ThreadPatternName = agentConfig.AgentSettings.FlowThreadPatternName,
                    PluginDirectory = "Plugins"
                });
        
            IStatsd statsd = Statsd.New<Udp>(options =>
            {
                options.Port = agentConfig.StatsdSettings.Port;
                options.HostOrIp = agentConfig.StatsdSettings.Host;
                options.Prefix = agentConfig.StatsdSettings.Prefix;
                options.BufferMetrics = false;
            });
            serviceCollection.AddSingleton<ICoreConfiguration>(agentConfig);
            serviceCollection.AddSingleton(statsd);
            var statsHandler = new StatsHandler(statsd);
            var loggerFactory = new LoggerFactory();
            var loggingHandler = new LoggingHandler(loggerFactory);

            serviceCollection.AddSingleton<IEventService>(new EventService(new List<IEventHandler>() { statsHandler, loggingHandler }));
        }
    }   
}
