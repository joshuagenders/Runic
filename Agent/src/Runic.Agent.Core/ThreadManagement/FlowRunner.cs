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
        private readonly CucumberHarness _harness;
        private readonly Flow _flow;
        public FlowRunner(FunctionFactory factory, CucumberHarness harness, Flow flow)
        {
            _factory = factory;
            _harness = harness;
            _flow = flow;
        }

        private async Task ExecuteFunctionAsync(CancellationToken ctx = default(CancellationToken))
        {
            FunctionHarness function = null;
            while (!ctx.IsCancellationRequested)
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
                await function.OrchestrateFunctionExecutionAsync(ctx);
            }
        }

        private async Task ExecuteCucumberAsync(CancellationToken ctx = default(CancellationToken))
        {
            while (!ctx.IsCancellationRequested)
            {
                foreach (var step in _flow.Steps)
                {
                    if (step.Cucumber != null)
                    {
                        await _harness.ExecuteTestAsync(step.Cucumber.AssemblyName, step.Cucumber.Document, ctx);
                    }
                }
            }
        }

        public async Task ExecuteFlowAsync(CancellationToken ctx = default(CancellationToken))
        {
            if (_flow.Steps.Any(s => !string.IsNullOrEmpty(s.Cucumber?.Document)))
            {
                //todo allow for mix of functions and cucumber execution in a flow
                await ExecuteCucumberAsync(ctx);
            }
            else
            {
                await ExecuteFunctionAsync(ctx);
            }
        }
    }
}
