using Microsoft.Extensions.DependencyInjection;
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
                await application.Run(cts.Token);
            }
        }
    }
}