using Runic.Agent.Console.Framework;
using Runic.Agent.ThreadManagement;
using System.Linq;

namespace Runic.Agent.Console.Pages
{
    public class ListRunningFlowsPage : Page
    {
        private readonly IThreadOrchestrator _threadOrchestrator;

        public ListRunningFlowsPage(MenuProgram program, IThreadOrchestrator threadOrchestrator) 
            : base("List Running Flows", program)
        {
            _threadOrchestrator = threadOrchestrator;
        }

        public override void Display()
        {
            base.Display();

            var flowId = Input.ReadString("Enter the flow id");
            _threadOrchestrator.GetRunningThreadPatterns()?.ToList().ForEach(p => Output.WriteLine(p));
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
