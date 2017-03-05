using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Runic.Agent.FlowManagement;
using Runic.Agent.Harness;
using Runic.Core.Models;

namespace Runic.Agent.UnitTest
{
    public class TestFlowHarness
    {
        [Test]
        public async Task TestSingleFunctionFlow()
        {
            var harness = new FlowHarness();
            var flow = new Flow()
            {
                Name = "ExampleFlow",
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        FunctionName = "Login",
                        FunctionAssemblyName = "ExampleTest.dll",
                        Repeat = 1
                    }
                }
            };

            Flows.AddUpdateFlow(flow);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(10000);
            var ex = harness.Execute(flow, new ThreadControl(1), cts.Token);
            Assert.True(harness.GetRunningThreadCount() == 1);
            cts.Cancel();
            await ex;
            Assert.True(harness.GetRunningThreadCount() == 0);
        }
    }
}
