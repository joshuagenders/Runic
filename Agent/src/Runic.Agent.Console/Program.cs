﻿using Autofac;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.FlowManagement;
using Runic.Agent.Service;

namespace Runic.Agent.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var startup = new Startup();
            var container = startup.BuildContainer(args);
            var service = container.Resolve<IAgentService>();
            var plugins = container.Resolve<IPluginManager>();
            var flowMgr = container.Resolve<IFlowManager>();

            new AgentConsole(service, flowMgr, plugins).Run();
        }
    }
}