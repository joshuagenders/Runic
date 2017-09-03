using Autofac;
using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadManagement;

namespace Runic.Agent.Core.IoC
{
    public class DefaultContainer
    {
        public static ContainerBuilder GetDefaultContainerBuilder()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<PluginManager>().As<IPluginManager>().SingleInstance();
            builder.RegisterType<FlowManager>().As<IFlowManager>().SingleInstance();
            //builder.RegisterType<PatternService>().As<IPatternService>().SingleInstance();
            builder.RegisterType<ThreadManager>().As<IThreadManager>().SingleInstance();
            builder.RegisterType<DateTimeService>().As<IDatetimeService>();
            builder.RegisterType<RunnerService>().As<IRunnerService>();

            return builder;
        }
    }
}
