using Runic.Agent.Console.Framework;
using Runic.Agent.Core.ThreadManagement;
using System.Linq;

namespace Runic.Agent.Console.Pages
{
    public class ListRunningThreadPatternsPage : Page
    {
        private readonly ThreadManager _threadManager;

        public ListRunningThreadPatternsPage(MenuProgram program, ThreadManager threadManager) 
            : base("List Running Thread Patterns", program)
        {
            _threadManager = threadManager;
        }

        public override void Display()
        {
            base.Display();

            var flowId = Input.ReadString("Enter the flow id");
            _threadManager.GetRunningFlows?.ToList().ForEach(f => Output.WriteLine(f));
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
