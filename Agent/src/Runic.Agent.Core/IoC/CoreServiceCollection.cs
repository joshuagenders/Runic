using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.WorkGenerator;

namespace Runic.Agent.Core.IoC
{
    public static class CoreServiceCollection
    {
        public static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFunctionFactory, FunctionFactory>();
            serviceCollection.AddTransient<IDatetimeService, DateTimeService>();
            serviceCollection.AddTransient<IPerson, Person>();
            serviceCollection.AddTransient<IPersonFactory, PersonFactory>();

            serviceCollection.AddSingleton<IAssemblyManager, AssemblyManager>();
            serviceCollection.AddSingleton<IConsumer<Work>, WorkConsumer>();
            serviceCollection.AddSingleton<IProducer<Work>, WorkProducer>();

            serviceCollection.AddScoped<IRunner<Work>>();
        }
    }
}
