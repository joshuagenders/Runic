using Runic.Agent.Console.Framework;
using Runic.Agent.Service;
using System.Linq;

namespace Runic.Agent.Console.Pages
{
    public class ListRunningThreadPatternsPage : Page
    {
        private readonly IAgentService _agentService;

        public ListRunningThreadPatternsPage(MenuProgram program, IAgentService agentService) 
            : base("List Running Thread Patterns", program)
        {
            _agentService = agentService;
        }

        public override void Display()
        {
            base.Display();

            var flowId = Input.ReadString("Enter the flow id");
            _agentService.GetRunningFlows().ToList().ForEach(f => Output.WriteLine(f));
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
