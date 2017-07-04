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
            if (AgentConfig.AgentSettings.DurationSeconds != 0)
            {
                cts.CancelAfter(AgentConfig.AgentSettings.DurationSeconds * 1000);
            }
            using (var scope = container.BeginLifetimeScope())
            {
                var agent = scope.Resolve<IApplication>();
                agent.RunApplicationAsync(cts.Token)
                     .ContinueWith((result) =>
                     {
                         if (result.Exception != null)
                         {
                             Console.WriteLine("An exception occured.");
                             Console.WriteLine(result.Exception.Message);
                         }
                         Console.WriteLine("Exiting application...");
                     })
                     .Wait();
            }
        }
    }
}