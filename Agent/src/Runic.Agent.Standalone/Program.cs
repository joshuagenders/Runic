using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace Runic.Agent.Standalone
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var serviceCollection = Startup.ConfigureServices(new ServiceCollection());
            
                var cts = new CancellationTokenSource();
                using (var scope = serviceCollection.BuildServiceProvider().CreateScope())
                {
                    var application = scope.ServiceProvider.GetService<IApplication>();
                    application.Run(cts.Token).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error has occured.");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Press the 'any' key to exit.");
                Console.ReadLine();
            }
        }
    }
}