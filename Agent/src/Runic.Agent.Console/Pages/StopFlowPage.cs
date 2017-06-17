using Runic.Agent.Console.Framework;
using Runic.Agent.ThreadManagement;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    public class StopFlowPage : Page
    {
        private readonly IThreadOrchestrator _threadOrchestrator;
        public StopFlowPage(MenuProgram program, IThreadOrchestrator threadOrchestrator) 
            : base("Stop Flow", program)
        {
            _threadOrchestrator = threadOrchestrator;
        }

        public override void Display()
        {
            base.Display();

            var flowId = Input.ReadString("Enter the flow id");
            var cts = new CancellationTokenSource();
            cts.CancelAfter(10000);
            _threadOrchestrator.StopFlow(flowId);
            Output.WriteLine(ConsoleColor.Green, $"Flow {flowId} thread level set to 0");
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}