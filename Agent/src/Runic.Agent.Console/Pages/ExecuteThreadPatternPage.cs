using Runic.Agent.Console.Framework;
using Runic.Agent.Service;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    public class ExecuteThreadPatternPage : Page
    {
        private readonly IAgentService _agentService;

        public ExecuteThreadPatternPage(MenuProgram program, IAgentService agentService)
            : base("Set Threads", program)
        {
            _agentService = agentService;
        }

        public override void Display()
        {
            base.Display();
            
            var flowId = Input.ReadString("Enter the flow id");
            //todo thread pattern
            var cts = new CancellationTokenSource();

            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
