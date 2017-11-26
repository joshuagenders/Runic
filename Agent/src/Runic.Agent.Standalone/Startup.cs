using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.IoC;
using Fclp;
using System;
using Runic.Agent.Core.Configuration;

namespace Runic.Agent.Standalone
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, params string[] args)
        {
            CoreServiceCollection.ConfigureServices(services);
            var configBuilder = GetConfigurationBuilder();
            var configResult = configBuilder.Parse(args);
            if (configResult.HasErrors)
            {
                throw new ArgumentException(configResult.ErrorText);
            }
            var config = configBuilder.Object;
            services.AddSingleton<ICoreConfiguration>(config);
            services.AddSingleton(config);
        }

        public static FluentCommandLineParser<Configuration> GetConfigurationBuilder()
        {
            var p = new FluentCommandLineParser<Configuration>();
            p.Setup(arg => arg.PluginFolderPath)
             .As('p', "pluginpath") 
             .Required();
            p.Setup(arg => arg.TaskCreationPollingIntervalSeconds)
             .As('i', "workpollingintervalseconds")
             .SetDefault(3);
            return p;
        }
    }
}
