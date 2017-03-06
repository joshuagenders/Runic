using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Runic.Agent.Configuration;
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
            var cli = new[]
            {
                "Agent:MaxThreads=321",
                "Agent:LifetimeSeconds=123",
                "Client:MQConnectionString=MyExampleConnection",
                "Statsd:Port=8000",
                "Statsd:Host=TestHost",
                "Statsd:Prefix=MyPrefix"
            };
            AgentConfiguration.LoadConfiguration(cli);
            Program.Container = new Startup().RegisterDependencies();

            var harness = new FlowHarness();
            var flow = new Flow()
            {
                Name = "ExampleFlow",
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        FunctionName = "Login",
                        FunctionAssemblyName = "ExampleTest"
                    }
                }
            };

            Flows.AddUpdateFlow(flow);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(10000);
            var ex = harness.Execute(flow, 1, cts.Token);
            var tasks = harness.GetTasks();
            Thread.Sleep(2000);

            Console.WriteLine(tasks.Count);
            Console.WriteLine(string.Join(",", tasks.Select(t => t.Status).ToList()));
            Assert.AreEqual(0, harness.GetSemaphoreCurrentCount());
            Assert.AreEqual(1, harness.GetRunningThreadCount());
            cts.Cancel();
            await ex;
            Assert.AreEqual(0, harness.GetRunningThreadCount());
        }
    }
}
