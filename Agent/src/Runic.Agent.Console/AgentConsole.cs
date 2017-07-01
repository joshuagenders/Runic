using Runic.Agent.Console.Framework;
using Runic.Agent.Console.Pages;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.ThreadManagement;

namespace Runic.Agent.Console
{
    public class AgentConsole : MenuProgram
    {
        public AgentConsole(IPatternService patternService,
                            IFlowManager flowManager,
                            IPluginManager pluginManager,
                            ThreadManager threadManager)
            : base("Runic Agent", breadcrumbHeader: true)
        {
            AddPage(new MainPage(this));            
            AddPage(new LoadFlowPage(this, flowManager));
            AddPage(new HelpPage(this));
            AddPage(new ListAssembliesPage(this, pluginManager));
            AddPage(new ListFunctionsPage(this, pluginManager));
            AddPage(new ListRunningFlowsPage(this, patternService));
            AddPage(new LoadAssemblyPage(this, pluginManager));
            AddPage(new LoadFlowPage(this, flowManager));
            AddPage(new SetThreadPage(this, threadManager));
            AddPage(new StopFlowPage(this, threadManager));
            AddPage(new DisplayAgentInformationPage(this));
            AddPage(new ExecuteThreadPatternPage(this));
            AddPage(new ExecuteConstantPatternPage(this, patternService, flowManager));
            AddPage(new ExecuteGraphPatternPage(this, patternService, flowManager));
            AddPage(new ExecuteGradualPatternPage(this, patternService, flowManager));
            AddPage(new StopThreadPatternPage(this, patternService));
            AddPage(new ListRunningThreadPatternsPage(this, threadManager));
            AddPage(new ExitPage(this));
            SetPage<MainPage>();
        }
    }
}
