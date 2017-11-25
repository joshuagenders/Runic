using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.WorkGenerator;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.IoC;
using Runic.Agent.Core.Services;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Runic.Agent.Standalone
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {

            CoreServiceCollection.ConfigureServices(services);
            services.AddScoped<IRunner<Expedition>>();
            services.AddTransient<IConsumer<Expedition>>((_) =>
            {
                return new ExpeditionConsumer(
                    new ConcurrentQueue<Expedition>(),
                    _.GetService<IPersonFactory>(),
                    _.GetService<IDatetimeService>());
            });
            services.AddTransient<IProducer<Expedition>>((_) =>
            {
                var producer = new ExpeditionProducer(
                    _.GetService<IConsumer<Expedition>>(),
                    _.GetService<IDatetimeService>(),
                    _.GetService<ICoreConfiguration>());

                foreach (var plan in GetTestPlans())
                {
                    producer.AddUpdateWorkItem(plan.Key, plan.Value);
                }

                return producer;
            });
        }

        public static Dictionary<string,Expedition> GetTestPlans()
        {
            //todo read from config
            return new Dictionary<string, Expedition>();
        }
    }
}
