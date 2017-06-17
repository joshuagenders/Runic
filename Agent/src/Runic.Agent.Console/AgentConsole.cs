using Runic.Agent.AssemblyManagement;
using Runic.Agent.Console.Framework;
using Runic.Agent.Console.Pages;
using Runic.Agent.FlowManagement;
using Runic.Agent.ThreadManagement;

namespace Runic.Agent.Console
{
    public class AgentConsole : MenuProgram
    {
        public AgentConsole(IThreadOrchestrator threadOrchestrator,
                            IFlowManager flowManager,
                            IPluginManager pluginManager)
            : base("Runic Agent", breadcrumbHeader: true)
        {
            AddPage(new MainPage(this));            
            AddPage(new LoadFlowPage(this, flowManager));
            AddPage(new HelpPage(this));
            AddPage(new ListAssembliesPage(this, pluginManager));
            AddPage(new ListFunctionsPage(this, pluginManager));
            AddPage(new ListRunningFlowsPage(this, threadOrchestrator));
            AddPage(new LoadAssemblyPage(this, pluginManager));
            AddPage(new LoadFlowPage(this, flowManager));
            AddPage(new SetThreadPage(this, threadOrchestrator));
            AddPage(new StopFlowPage(this, threadOrchestrator));
            AddPage(new DisplayAgentInformationPage(this));
            AddPage(new ExecuteThreadPatternPage(this));
            AddPage(new ExecuteConstantPatternPage(this, threadOrchestrator, flowManager));
            AddPage(new ExecuteGraphPatternPage(this, threadOrchestrator, flowManager));
            AddPage(new ExecuteGradualPatternPage(this, threadOrchestrator, flowManager));
            AddPage(new StopThreadPatternPage(this, threadOrchestrator));
            AddPage(new ListRunningThreadPatternsPage(this, threadOrchestrator));
            AddPage(new ExitPage(this));
            SetPage<MainPage>();
        }
    }
}
