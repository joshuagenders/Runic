using Runic.Agent.Console.Framework;
using Runic.Agent.Service;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    public class StopThreadPatternPage : Page
    {
        private readonly IAgentService _agentService;
        public StopThreadPatternPage(MenuProgram program, IAgentService agentService) 
            : base("Stop Thread Pattern", program)
        {
            _agentService = agentService;
        }

        public override void Display()
        {
            base.Display();

            var patternId = Input.ReadString("Enter the thread pattern id");
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            _agentService.StopPattern(patternId);
            Output.WriteLine(ConsoleColor.Green, $"Thread pattern {patternId} stopped");
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
