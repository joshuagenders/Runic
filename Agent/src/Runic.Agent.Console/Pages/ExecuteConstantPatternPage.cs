using Runic.Agent.Console.Framework;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Framework.Models;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    public class ExecuteConstantPatternPage : Page
    {
        private readonly IPatternService _threadOrchestrator;
        private readonly IFlowManager _flowManager;
        
        public ExecuteConstantPatternPage(MenuProgram program, IPatternService threadOrchestrator, IFlowManager flowManager)
            : base("Execute Constant Pattern", program)
        {
            _threadOrchestrator = threadOrchestrator;
            _flowManager = flowManager;
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
                cts.CancelAfter(flowRequest.ThreadPattern.DurationSeconds * 1000);
                _threadOrchestrator.StartThreadPattern(
                    flowRequest.PatternExecutionId, 
                    flowRequest.Flow, 
                    new ConstantThreadPattern()
                    {
                        DurationSeconds = flowRequest.ThreadPattern.DurationSeconds,
                        ThreadCount = flowRequest.ThreadPattern.ThreadCount
                    }, cts.Token);
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
