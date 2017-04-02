using Autofac;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.FlowManagement;
using Runic.Agent.Service;

namespace Runic.Agent.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AgentConfiguration.LoadConfiguration(args);
            var startup = new Startup();
            var container = startup.BuildContainer();
            var service = container.Resolve<IAgentService>();
            var plugins = container.Resolve<IPluginManager>();
            var flowMgr = container.Resolve<IFlowManager>();
            //todo configure interactive mode?
            new AgentConsole(service, flowMgr, plugins).Run();
        }
    }
}