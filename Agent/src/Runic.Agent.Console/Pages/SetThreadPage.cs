using Runic.Agent.Console.Framework;
using Runic.Agent.Service;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    class SetThreadPage : Page
    {
        private readonly IAgentService _agentService;

        public SetThreadPage(MenuProgram program, IAgentService agentService)
            : base("Set Threads", program)
        {
            _agentService = agentService;
        }

        public override void Display()
        {
            base.Display();
            
            var flowId = Input.ReadString("Enter the flow id");
            var threadLevel = Input.ReadInt("Enter the thread level", 0, 1000);
            var cts = new CancellationTokenSource();
            _agentService.SetThreadLevel(new Runic.Framework.Models.SetThreadLevelRequest()
            {
                FlowName = flowId,
                ThreadLevel = threadLevel
            }, cts.Token).GetAwaiter().GetResult();
            Output.WriteLine(ConsoleColor.Green, $"Flow {flowId} thread level set to {threadLevel}");
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
