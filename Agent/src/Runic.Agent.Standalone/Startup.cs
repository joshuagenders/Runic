using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.IoC;
using Fclp;
using System;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.WorkGenerator;
using System.Collections.Concurrent;

namespace Runic.Agent.Standalone
{
    public static class Startup
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services, params string[] args)
        {
            CoreServiceCollection.ConfigureServices(services);
            services.AddSingleton<IConfigBuilder>((_) => new ConfigBuilder(args));
            services.AddSingleton<ICoreConfiguration>((_) => _.GetService<IConfigBuilder>().Config);
            services.AddSingleton((_) => _.GetService<IConfigBuilder>().Config);
            services.AddSingleton<IProducerConsumerCollection<Work>, ConcurrentQueue<Work>>();
            services.AddSingleton<IWorkLoader, WorkLoader>();
            services.AddTransient<IApplication, Application>();
            return services;
        }
    }
}
