using Runic.Agent.Console.Framework;
using Runic.Agent.ThreadManagement;
using System.Linq;

namespace Runic.Agent.Console.Pages
{
    public class ListRunningThreadPatternsPage : Page
    {
        private readonly IThreadOrchestrator _threadOrchestrator;

        public ListRunningThreadPatternsPage(MenuProgram program, IThreadOrchestrator threadOrchestrator) 
            : base("List Running Thread Patterns", program)
        {
            _threadOrchestrator = threadOrchestrator;
        }

        public override void Display()
        {
            base.Display();

            var flowId = Input.ReadString("Enter the flow id");
            _threadOrchestrator.GetRunningFlows?.ToList().ForEach(f => Output.WriteLine(f));
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
