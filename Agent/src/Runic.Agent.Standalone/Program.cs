using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.WorkGenerator;
using System.Threading;

namespace Runic.Agent.Standalone
{
    static class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            Startup.ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var cts = new CancellationTokenSource();
            using (var scope = serviceProvider.CreateScope())
            {
                var runner = serviceProvider.GetRequiredService<IRunner<Work>>();
                runner.Start(cts.Token).Wait();
            }
        }
    }
}