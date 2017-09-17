using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Standalone.Configuration;
using System;
using System.Threading;

namespace Runic.Agent.Standalone
{
    static class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            Startup.ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var cts = new CancellationTokenSource();
            
            using (var scope = serviceProvider.CreateScope())
            {
                var config = scope.ServiceProvider.GetService<IAgentConfig>();
                if (config.AgentSettings.FlowDurationSeconds != 0)
                {
                    cts.CancelAfter(config.AgentSettings.FlowDurationSeconds * 1000);
                }

                var agent = scope.ServiceProvider.GetService<IApplication>();
                agent.RunApplicationAsync(cts.Token)
                     .ContinueWith((result) =>
                     {
                         if (result.Exception != null)
                         {
                             Console.WriteLine("An exception occured.");
                             Console.WriteLine(result.Exception.Message);
                         }
                         else
                         {
                             Console.WriteLine("Execution complete.");
                         }
                         Console.WriteLine("Exiting application...");
                         Console.ReadLine();
                     })
                     .Wait();
            }
        }
    }
}