using Autofac;
using Runic.Agent.Standalone.Configuration;
using System;
using System.Threading;

namespace Runic.Agent.Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            var startup = new Startup();
            var container = startup.BuildContainer();
            var cts = new CancellationTokenSource();
            
            using (var scope = container.BeginLifetimeScope())
            {
                var config = scope.Resolve<IAgentConfig>();
                if (config.AgentSettings.FlowDurationSeconds != 0)
                {
                    cts.CancelAfter(config.AgentSettings.FlowDurationSeconds * 1000);
                }

                var agent = scope.Resolve<IApplication>();
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