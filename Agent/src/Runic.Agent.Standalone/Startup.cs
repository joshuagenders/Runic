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
            services.AddScoped<IRunner<TestPlan>>();
            services.AddTransient<IConsumer<TestPlan>>((_) =>
            {
                return new TestPlanConsumer(
                    new ConcurrentQueue<TestPlan>(),
                    _.GetService<IPersonFactory>(),
                    _.GetService<IDatetimeService>());
            });
            services.AddTransient<IProducer<TestPlan>>((_) =>
            {
                var producer = new TestPlanProducer(
                    _.GetService<IConsumer<TestPlan>>(),
                    _.GetService<IDatetimeService>(),
                    _.GetService<ICoreConfiguration>());

                foreach (var plan in GetTestPlans())
                {
                    producer.AddUpdateWorkItem(plan.Key, plan.Value);
                }

                return producer;
            });
        }

        public static Dictionary<string,TestPlan> GetTestPlans()
        {
            //todo read from config
            return new Dictionary<string, TestPlan>();
        }
    }
}
