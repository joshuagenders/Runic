using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.IoC;
using Runic.Agent.Core.WorkGenerator;
using Runic.Agent.Standalone;

namespace Runic.Agent.FunctionalTest.TestUtility
{
    public class TestEnvironmentBuilder
    {
        public TestEnvironment Build(string [] args = null, TestEnvironment testEnvironment = null)
        {
            testEnvironment = testEnvironment ?? new TestEnvironment();

            args = args ?? new [] { "-p", "/plugins", "-w", "/work", "-i", "3" };
            var config = Startup.GetConfig(args);

            var services = new ServiceCollection();
            CoreServiceCollection.ConfigureServices(services);
            services.AddSingleton<ICoreConfiguration>(config);
            services.AddSingleton(config);
            services.AddSingleton(testEnvironment.WorkLoader ?? new WorkLoader());
            services.AddSingleton<IApplication>();

            if (testEnvironment?.AssemblyManager != null)
            {
                services.AddSingleton(testEnvironment.AssemblyManager);
            }
            if (testEnvironment?.WorkConsumer != null)
            {
                services.AddSingleton(testEnvironment.WorkConsumer);
            }
            if (testEnvironment?.WorkProducer != null)
            {
                services.AddSingleton(testEnvironment.WorkProducer);
            }

            var provider = services.BuildServiceProvider();
            testEnvironment.AssemblyManager = provider.GetService<IAssemblyManager>();
            testEnvironment.WorkConsumer = provider.GetService<IConsumer<Work>>();
            testEnvironment.WorkProducer = provider.GetService<IProducer<Work>>();
            testEnvironment.WorkLoader = provider.GetService<IWorkLoader>();
            testEnvironment.Runner = provider.GetService<IRunner<Work>>();
            testEnvironment.Application = provider.GetService<IApplication>();

            return testEnvironment;
        }
    }
}
