using Runic.Agent.Console.Framework;
using Runic.Agent.Service;
using Runic.Framework.Models;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    public class StopFlowPage : Page
    {
        private readonly IAgentService _agentService;
        public StopFlowPage(MenuProgram program, IAgentService agentService) 
            : base("Stop Flow", program)
        {
            _agentService = agentService;
        }

        public override void Display()
        {
            base.Display();

            var flowId = Input.ReadString("Enter the flow id");
            var cts = new CancellationTokenSource();
            cts.CancelAfter(10000);
            _agentService.StopFlow(flowId);
            Output.WriteLine(ConsoleColor.Green, $"Flow {flowId} thread level set to 0");
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}