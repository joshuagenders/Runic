using Runic.Agent.Console.Framework;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Framework.Models;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    public class ExecuteGradualPatternPage : MenuPage
    {
        private readonly IPatternService _threadOrchestrator;
        private readonly IFlowManager _flowManager;
        
        public ExecuteGradualPatternPage(MenuProgram program, IPatternService threadOrchestrator, IFlowManager flowManager)
            : base("Execute Gradual Pattern", program)
        {
            _threadOrchestrator = threadOrchestrator;
            _flowManager = flowManager;
        }

        public override void Display()
        {
            base.Display();

            var cts = new CancellationTokenSource();

            var flowRequest = new GradualFlowExecutionRequest();
            //todo validation
            try
            {
                flowRequest.Flow = _flowManager.GetFlow(Input.ReadString("Please enter the flow name"));
                flowRequest.PatternExecutionId = Guid.NewGuid().ToString("N");
                flowRequest.ThreadPattern = new GradualThreadModel()
                {
                    DurationSeconds = Input.ReadInt("Duration Seconds", 0, int.MaxValue),
                    ThreadCount = Input.ReadInt("Thread Count", 0, 1000),
                    RampDownSeconds = Input.ReadInt("Rampdown Seconds", 0, int.MaxValue),
                    RampUpSeconds = Input.ReadInt("Rampup Seconds", 0, int.MaxValue),
                    StepIntervalSeconds = Input.ReadInt("Step Interval Seconds", 0, 3600)
                };
                cts.CancelAfter(flowRequest.ThreadPattern.DurationSeconds * 1000);
                _threadOrchestrator.StartThreadPattern(flowRequest.PatternExecutionId, flowRequest.Flow, new GradualThreadPattern()
                {
                    DurationSeconds = flowRequest.ThreadPattern.DurationSeconds,
                    Points = flowRequest.ThreadPattern.Points,
                    RampDownSeconds= flowRequest.ThreadPattern.RampDownSeconds,
                    RampUpSeconds = flowRequest.ThreadPattern.RampUpSeconds,
                    StepIntervalSeconds = flowRequest.ThreadPattern.StepIntervalSeconds,
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
