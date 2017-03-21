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
        private AgentWorld _world { get; set; }

        [TestInitialize]
        public void Init()
        {
            _world = new AgentWorld();
        }

        [TestMethod]
        public async Task TestSingleFunctionFlow()
        {
            var harness = new FlowHarness(_world.PluginManager);

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
