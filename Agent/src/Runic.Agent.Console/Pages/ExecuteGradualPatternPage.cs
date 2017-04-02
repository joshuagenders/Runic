using Runic.Agent.Console.Framework;
using Runic.Agent.FlowManagement;
using Runic.Agent.Service;
using Runic.Framework.Models;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    public class ExecuteGradualPatternPage : MenuPage
    {
        private readonly IAgentService _agentService;
        private readonly IFlowManager _flowManager;

        public ExecuteGradualPatternPage(MenuProgram program, IAgentService agentService, IFlowManager flowManager)
            : base("Execute Gradual Pattern", program)
        {
            _agentService = agentService;
            _flowManager = flowManager;
        }

        public override void Display()
        {
            base.Display();

            var cts = new CancellationTokenSource();

            var flowRequest = new GradualFlowExecutionRequest();
            //todo validation
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
            cts.CancelAfter(5000);
            _agentService.ExecuteFlow(flowRequest, cts.Token);

            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
