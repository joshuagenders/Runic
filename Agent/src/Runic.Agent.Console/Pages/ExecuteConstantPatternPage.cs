using Runic.Agent.Console.Framework;
using Runic.Agent.FlowManagement;
using Runic.Agent.Services;
using Runic.Agent.ThreadManagement;
using Runic.Framework.Models;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    public class ExecuteConstantPatternPage : Page
    {
        private readonly IThreadOrchestrator _threadOrchestrator;
        private readonly IFlowManager _flowManager;
        private readonly ConstantFlowService _flowService;

        public ExecuteConstantPatternPage(MenuProgram program, IThreadOrchestrator threadOrchestrator, IFlowManager flowManager)
            : base("Execute Constant Pattern", program)
        {
            _threadOrchestrator = threadOrchestrator;
            _flowManager = flowManager;
            _flowService = new ConstantFlowService(_threadOrchestrator);
        }

        public override void Display()
        {
            base.Display();
            
            var cts = new CancellationTokenSource();

            var flowRequest = new ConstantFlowExecutionRequest();
            try
            {
                //todo validation
                flowRequest.Flow = _flowManager.GetFlow(Input.ReadString("Please enter the flow name"));
                flowRequest.PatternExecutionId = Guid.NewGuid().ToString("N");
                flowRequest.ThreadPattern = new ConstantThreadModel()
                {
                    DurationSeconds = Input.ReadInt("Duration Seconds", 0, int.MaxValue),
                    ThreadCount = Input.ReadInt("Thread Count", 0, 1000)
                };
                cts.CancelAfter(5000);
                _flowService.ExecuteFlow(flowRequest, cts.Token);
            }
            catch (Exception e)
            {
                Output.WriteLine(ConsoleColor.Red, $"An error occured: {e.Message}");
            }
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
