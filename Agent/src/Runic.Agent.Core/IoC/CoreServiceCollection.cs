using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Agent.TestHarness.Harness;
using Runic.Agent.TestHarness.Services;

namespace Runic.Agent.Core.IoC
{
    public static class CoreServiceCollection
    {
        public static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDataService, DataService>();
            serviceCollection.AddTransient<IFunctionFactory, FunctionFactory>();
            serviceCollection.AddTransient<IPluginManager, PluginManager>();
            serviceCollection.AddTransient<IEventService, EventService>();
            serviceCollection.AddTransient<IThreadManager, ThreadManager>();
            serviceCollection.AddTransient<IFlowPatternController, FlowPatternController>();
            serviceCollection.AddTransient<IFunctionFactory, FunctionFactory>();
            serviceCollection.AddTransient<IDatetimeService, DateTimeService>();
            serviceCollection.AddTransient<IRunnerService, RunnerService>();
        }
    }
}
