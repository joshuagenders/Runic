using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.IoC;
using Fclp;
using System;
using Runic.Agent.Core.Configuration;

namespace Runic.Agent.Standalone
{
    public static class Startup
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services, params string[] args)
        {
            var config = GetConfig(args);

            CoreServiceCollection.ConfigureServices(services);
            services.AddSingleton<ICoreConfiguration>(config);
            services.AddSingleton(config);
            services.AddSingleton<IWorkLoader>(new WorkLoader());
            services.AddTransient<IApplication>();
            return services;
        }

        public static Configuration GetConfig(string[] args)
        {
            var configBuilder = GetConfigurationBuilder();
            var configResult = configBuilder.Parse(args);
            if (configResult.HasErrors)
            {
                throw new ArgumentException(configResult.ErrorText);
            }
            return configBuilder.Object;
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
            p.Setup(arg => arg.WorkFolderPath)
             .As('w', "workpath")
             .Required();
            return p;
        }
    }
}
