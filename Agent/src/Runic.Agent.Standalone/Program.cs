using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.WorkGenerator;
using System.Linq;
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
                var configuration = serviceProvider.GetRequiredService<Configuration>();
                var producer = serviceProvider.GetRequiredService<IProducer<Work>>();
                WorkLoader.GetWork(configuration)
                          .ToList()
                          .ForEach(i => producer.AddUpdateWorkItem(i.Journey.Name, i));

                var runner = serviceProvider.GetRequiredService<IRunner<Work>>();
                await runner.Start(cts.Token);
            }
        }
    }
}