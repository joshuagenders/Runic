using Runic.Agent.Console.Framework;
using Runic.Agent.Service;
using System.Linq;

namespace Runic.Agent.Console.Pages
{
    public class ListRunningFlowsPage : Page
    {
        private readonly IAgentService _agentService;

        public ListRunningFlowsPage(MenuProgram program, IAgentService agentService) 
            : base("List Running Flows", program)
        {
            _agentService = agentService;
        }

        public override void Display()
        {
            base.Display();

            var flowId = Input.ReadString("Enter the flow id");
            _agentService.GetRunningThreadPatterns()?.ToList().ForEach(p => Output.WriteLine(p));
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
