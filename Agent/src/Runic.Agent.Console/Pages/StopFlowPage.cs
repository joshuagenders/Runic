using Runic.Agent.Console.Framework;
using Runic.Agent.Core.ThreadManagement;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    public class StopFlowPage : Page
    {
        private readonly ThreadManager _threadManager;
        public StopFlowPage(MenuProgram program, ThreadManager threadManager) 
            : base("Stop Flow", program)
        {
            _threadManager = threadManager;
        }

        public override void Display()
        {
            base.Display();

            var flowId = Input.ReadString("Enter the flow id");
            var cts = new CancellationTokenSource();
            cts.CancelAfter(10000);
            _threadManager.StopFlow(flowId);
            Output.WriteLine(ConsoleColor.Green, $"Flow {flowId} thread level set to 0");
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}