using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Configuration;
using Runic.Agent.Harness;
using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.AssemblyManagement;

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
            var container = new Startup().Register();

            var pluginManager = new PluginManager();
            pluginManager.RegisterProvider(new FilePluginProvider(wd));
            var harness = new FlowHarness(pluginManager);

            var flow = new Flow()
            {
                Name = "ExampleFlow",
                StepDelayMilliseconds = 200,
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step1",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.ExampleTest",
                            AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.FakeFunction",
                            FunctionName = "FakeFunction"
                        },
                        NextStepOnFailure = "Step1",
                        NextStepOnSuccess = "Step1",
                        Repeat = 2
                    }
                }
            };

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(600);
            var task = harness.Execute(flow, 1, cts.Token);
            Thread.Sleep(50);
            //todo
            Assert.AreEqual(0, harness.GetSemaphoreCurrentCount());
            Assert.IsTrue(harness.GetTotalInitiatiedThreadCount() == 1);
            
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
