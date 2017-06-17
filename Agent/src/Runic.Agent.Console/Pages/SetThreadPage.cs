using Runic.Agent.Console.Framework;
using Runic.Agent.ThreadManagement;
using Runic.Framework.Models;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    class SetThreadPage : Page
    {
        private readonly IThreadOrchestrator _threadOrchestrator;

        public SetThreadPage(MenuProgram program, IThreadOrchestrator threadOrchestrator)
            : base("Set Threads", program)
        {
            _threadOrchestrator = threadOrchestrator;
        }

        public override void Display()
        {
            base.Display();
            
            var flowId = Input.ReadString("Enter the flow id");
            var threadLevel = Input.ReadInt("Enter the thread level", 0, 1000);
            var cts = new CancellationTokenSource();
            _threadOrchestrator.SetThreadLevelAsync(new SetThreadLevelRequest()
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
