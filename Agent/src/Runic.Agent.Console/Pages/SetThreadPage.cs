using Runic.Agent.Console.Framework;
using Runic.Agent.Core.ThreadManagement;
using Runic.Framework.Models;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    class SetThreadPage : Page
    {
        private readonly ThreadManager _threadManager;

        public SetThreadPage(MenuProgram program, ThreadManager threadManager)
            : base("Set Threads", program)
        {
            _threadManager = threadManager;
        }

        public override void Display()
        {
            base.Display();
            
            var flowId = Input.ReadString("Enter the flow id");
            var threadLevel = Input.ReadInt("Enter the thread level", 0, 1000);
            var cts = new CancellationTokenSource();
            _threadManager.SetThreadLevelAsync(new SetThreadLevelRequest()
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
