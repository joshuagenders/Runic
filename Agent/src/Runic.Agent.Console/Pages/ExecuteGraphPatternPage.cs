using Runic.Agent.Console.Framework;
using Runic.Agent.FlowManagement;
using Runic.Agent.Service;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    public class ExecuteGraphPatternPage : Page
    {
        private readonly IAgentService _agentService;
        private readonly IFlowManager _flowManager;

        public ExecuteGraphPatternPage(MenuProgram program, IAgentService agentService, IFlowManager flowManager)
            : base("Execute Graph Pattern", program)
        {
            _agentService = agentService;
            _flowManager = flowManager;
        }

        public override void Display()
        {
            base.Display();
            
            
            var cts = new CancellationTokenSource();

            var flowRequest = new GraphFlowExecutionRequest();
            //todo validation
            try
            { 
                flowRequest.Flow = _flowManager.GetFlow(Input.ReadString("Please enter the flow name"));
                flowRequest.PatternExecutionId = Guid.NewGuid().ToString("N");
                var points = new List<Point>();

                int unitsFromStart = 0;
                int threadLevel = 0;
                do
                {
                    unitsFromStart = Input.ReadInt("Enter point units from start, or 0 to finish", 0, 1000);
                    if (unitsFromStart == 0)
                        break;
                    threadLevel = Input.ReadInt("Enter point thread level", 0, 1000);
                    points.Add(new Point() { unitsFromStart = unitsFromStart, threadLevel = threadLevel });
                } while (unitsFromStart > 0);
                points.Add(new Point() { unitsFromStart = points.Max(p=>p.unitsFromStart) + 1, threadLevel = 0 });

                flowRequest.ThreadPattern = new GraphThreadModel()
                {
                    DurationSeconds = Input.ReadInt("Duration Seconds", 0, int.MaxValue),
                    Points = points
                };
                cts.CancelAfter(flowRequest.ThreadPattern.DurationSeconds * 1000 + 200);
                _agentService.ExecuteFlow(flowRequest, cts.Token);
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
