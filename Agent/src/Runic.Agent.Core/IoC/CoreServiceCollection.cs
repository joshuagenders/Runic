using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Services;

namespace Runic.Agent.Core.IoC
{
    public static class CoreServiceCollection
    {
        public static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFunctionFactory, FunctionFactory>();
            serviceCollection.AddTransient<IAssemblyManager, AssemblyManager>();
            serviceCollection.AddTransient<IFunctionFactory, FunctionFactory>();
            serviceCollection.AddTransient<IDatetimeService, DateTimeService>();
            serviceCollection.AddTransient<IPerson, Person>();
        }
    }
}
