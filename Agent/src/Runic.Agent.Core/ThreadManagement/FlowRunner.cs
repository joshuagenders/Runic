using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Harness;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadManagement
{
    public class FlowRunner
    {
        private readonly FunctionFactory _factory;
        private readonly Flow _flow;
        public FlowRunner(FunctionFactory factory, Flow flow)
        {
            _factory = factory;
            _flow = flow;
        }

        public async Task ExecuteFlowAsync(CancellationToken ct)
        {
            FunctionHarness function = null;
            while (!ct.IsCancellationRequested)
            {
                if (function == null)
                {
                    function = _factory.CreateFunction(_flow.Steps.First());
                }
                else if (function?.NextStep != null)
                {
                    function = _factory.CreateFunction(function.NextStep);
                }
                else
                {
                    int functionIndex = _flow.Steps
                                             .Where(s => s.StepName == function.StepName)
                                             .Select(s => _flow.Steps.IndexOf(s))
                                             .Single();
                    functionIndex++;
                    functionIndex = functionIndex >= _flow.Steps.Count ? 0 : functionIndex;

                    function = _factory.CreateFunction(_flow.Steps[functionIndex]);
                }
                await function.OrchestrateFunctionExecutionAsync(ct);
            }
        }
    }
}
