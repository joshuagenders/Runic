using Autofac;
using Runic.Agent.Worker.Configuration;
using Moq;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Worker.Services;
using Runic.Agent.Worker.Clients;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Worker.Messaging;
using Runic.Framework.Clients;
using System.IO;
using Microsoft.Extensions.Logging;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services.Interfaces;
using Runic.Agent.Core.Configuration;

namespace Runic.Agent.Worker.Test.TestUtility
{
    public class TestStartup : IStartup
    {
        public IContainer BuildContainer(string[] args = null)
        {
            WorkerConfiguration.LoadConfiguration(args);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(new Mock<IDataService>().Object).As<IDataService>();
            builder.RegisterInstance(new Mock<IStatsClient>().Object).As<IStatsClient>();
            builder.RegisterInstance(new Mock<ILoggingHandler>().Object).As<ILoggingHandler>();

            builder.RegisterType<FilePluginProvider>()
                   .WithParameter(new PositionalParameter(0, Directory.GetCurrentDirectory()))
                   .As<IPluginProvider>();
            builder.Register<InMemoryRuneClient, IRuneClient>()
                   .Register<PluginManager, IPluginManager>()       
                   .Register<HandlerRegistry, IHandlerRegistry>()
                   .Register<MessagingDataService, IDataService>()
                   .Register<DateTimeService, IDatetimeService>()
                   .Register<TestEnvironment, IApplication>()
                   .Register<LoggerFactory, ILoggerFactory>()
                   .Register<FunctionFactory, IFunctionFactory>()
                   .RegisterInstance(new AgentCoreConfiguration()
                   {
                       MaxThreads = 10
                   });

            var agentObserver = new AgentObserver();
            builder.RegisterInstance(agentObserver).As<IAgentObserver>();

            builder.RegisterType<PatternService>().As<IPatternService>().SingleInstance();
            builder.RegisterType<FlowManager>().As<IFlowManager>().SingleInstance();
            builder.RegisterType<InMemoryMessagingService>().As<IMessagingService>().SingleInstance();
            builder.RegisterType<ThreadManager>().As<IThreadManager>().SingleInstance();
            builder.RegisterType<RunnerService>().As<IRunnerService>();
            builder.RegisterType<TestResultHandlerService>().As<ITestResultHandler>();
            return builder.Build();
        }
    }
}
