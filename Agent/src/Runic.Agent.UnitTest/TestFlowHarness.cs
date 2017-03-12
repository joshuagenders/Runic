using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Configuration;
using Runic.Agent.Harness;
using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestFlowHarness
    {
        [TestMethod]
        public async Task TestSingleFunctionFlow()
        {
            var cli = new[]
            {
                "Agent:MaxThreads=321",
                "Agent:LifetimeSeconds=123",
                "Client:MQConnectionString=MyExampleConnection",
                "Statsd:Port=8125",
                "Statsd:Host=localhost",
                "Statsd:Prefix=Runic.Stats."
            };
            AgentConfiguration.LoadConfiguration(cli);
            IoC.RegisterDependencies(new Startup());

            var harness = new FlowHarness();
            var flow = new Flow()
            {
                Name = "ExampleFlow",
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        FunctionName = "Login",
                        FunctionAssemblyName = "ExampleTest",
                        FunctionFullyQualifiedName = "Runic.ExampleTest.Functions.FakeFunction"
                    }
                }
            };

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(600);
            var task = harness.Execute(flow, 1, cts.Token).ConfigureAwait(false);
            Thread.Sleep(50);
            Assert.AreEqual(0, harness.GetSemaphoreCurrentCount());
            Assert.IsTrue(harness.GetTotalInitiatiedThreadCount() > 0);
            
            try
            {
                cts.Cancel();
                await task;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Caught TaskCanceledException");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Caught OperationCanceledException");
            }
        }
    }
}
