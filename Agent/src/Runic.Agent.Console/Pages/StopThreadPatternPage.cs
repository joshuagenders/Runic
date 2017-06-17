using Runic.Agent.Console.Framework;
using Runic.Agent.ThreadManagement;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    public class StopThreadPatternPage : Page
    {
        private readonly IThreadOrchestrator _threadOrchestrator;
        public StopThreadPatternPage(MenuProgram program, IThreadOrchestrator threadOrchestrator) 
            : base("Stop Thread Pattern", program)
        {
            _threadOrchestrator = threadOrchestrator;
        }

        public override void Display()
        {
            base.Display();

            var patternId = Input.ReadString("Enter the thread pattern id");
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            _threadOrchestrator.StopPattern(patternId);
            Output.WriteLine(ConsoleColor.Green, $"Thread pattern {patternId} stopped");
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
