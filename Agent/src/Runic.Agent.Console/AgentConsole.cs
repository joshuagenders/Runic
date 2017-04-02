using Runic.Agent.AssemblyManagement;
using Runic.Agent.Console.Framework;
using Runic.Agent.Console.Pages;
using Runic.Agent.FlowManagement;
using Runic.Agent.Service;

namespace Runic.Agent.Console
{
    public class AgentConsole : MenuProgram
    {
        public AgentConsole(IAgentService agentService,
                            IFlowManager flowManager,
                            IPluginManager pluginManager)
            : base("Runic Agent", breadcrumbHeader: true)
        {
            AddPage(new MainPage(this));            
            AddPage(new LoadFlowPage(this, flowManager));
            AddPage(new HelpPage(this));
            AddPage(new ListAssembliesPage(this, pluginManager));
            AddPage(new ListFunctionsPage(this, pluginManager));
            AddPage(new ListRunningFlowsPage(this, agentService));
            AddPage(new LoadAssemblyPage(this, pluginManager));
            AddPage(new LoadFlowPage(this, flowManager));
            AddPage(new SetThreadPage(this, agentService));
            AddPage(new StopFlowPage(this, agentService));
            AddPage(new DisplayAgentInformationPage(this));
            AddPage(new ExecuteThreadPatternPage(this));
            AddPage(new ExecuteConstantPatternPage(this, agentService, flowManager));
            AddPage(new ExecuteGraphPatternPage(this, agentService, flowManager));
            AddPage(new ExecuteGradualPatternPage(this, agentService, flowManager));
            AddPage(new StopThreadPatternPage(this, agentService));
            AddPage(new ListRunningThreadPatternsPage(this, agentService));
            AddPage(new ExitPage(this));
            SetPage<MainPage>();
        }
    }
}
