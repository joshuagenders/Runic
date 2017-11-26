using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.WorkGenerator;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone
{
    public static class Program
    {
       public static async Task Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            Startup.ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var cts = new CancellationTokenSource();
            using (var scope = serviceProvider.CreateScope())
            {
                var runner = serviceProvider.GetRequiredService<IRunner<Work>>();
                await runner.Start(cts.Token);
            }
        }
    }
}