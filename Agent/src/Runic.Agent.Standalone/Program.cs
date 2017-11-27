using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceCollection = Startup.ConfigureServices(new ServiceCollection());
            
            var cts = new CancellationTokenSource();
            using (var scope = serviceCollection.BuildServiceProvider().CreateScope())
            {
                var application = scope.ServiceProvider.GetService<IApplication>();
                try
                {
                    await application.Run(cts.Token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error has occured.");
                    Console.WriteLine(JsonConvert.SerializeObject(ex));
                }
                finally
                {
                    Console.WriteLine("Press the 'any' key to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}