using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Configuration;
using Runic.Agent.Harness;
using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.AssemblyManagement;
using System.IO;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestFlowHarness
    {
        //bad hack because nunit 3 doesn't work yet and mstest doesnt set cwd to deployment dir
        // and mstestv2 test context has removed the deployment metadata
        private const string wd = "C:\\code\\Runic\\Agent\\src\\Runic.Agent.UnitTest\\bin\\Debug\\netcoreapp1.0";

        [TestMethod]
        public async Task TestSingleFunctionFlow()
        {
            var cli = new[]
            {
                "Agent:MaxThreads=321",
                "Agent:LifetimeSeconds=123",
                "Client:MQConnectionString=MyExampleConnection",
                "Statsd:Port=8125",
                "Statsd:Host=192.168.99.100",
                "Statsd:Prefix=Runic.Stats."
            };
            AgentConfiguration.LoadConfiguration(cli);
            var container = new Startup().RegisterDependencies();

            var harness = new FlowHarness(new FilePluginProvider(wd));
            var flow = new Flow()
            {
                Name = "ExampleFlow",
                StepDelayMilliseconds = 200,
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "ExampleTest",
                            AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.FakeFunction",
                            FunctionName = "FakeFunction"
                        }
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
