using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.TestHarness.Harness;
using Runic.Agent.TestHarness.Services;

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
            serviceCollection.AddTransient<IPersonFactory, PersonFactory>();
            serviceCollection.AddTransient<IPerson, Person>();
        }
    }
}
