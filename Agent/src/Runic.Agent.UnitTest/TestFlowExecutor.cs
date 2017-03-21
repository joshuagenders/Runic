using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.Harness;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestFlowExecutor
    {
        private AgentWorld _world { get; set; }

        [TestInitialize]
        public void Init()
        {
            _world = new AgentWorld();
        }

        [TestMethod]
        public async Task TestSingleExecute()
        {
            var flow = new Flow()
            {
                Name = "Test",
                StepDelayMilliseconds = 100,
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step1",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.ExampleTest",
                            FunctionName = "AsyncWait",
                            AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.FakeFunction"
                        },
                        Repeat = 1,
                        EvaluateSuccessOnRepeat = false //todo
                    }
                }
            };

            var flowInit = new FlowInitialiser(_world.PluginManager);
            flowInit.InitialiseFlow(flow);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(500);
            var flowExecutor = new FlowExecutor(flow, _world.PluginManager);
            var flowTask = flowExecutor.ExecuteFlow(cts.Token);
            await flowTask;
            Assert.IsTrue(flowTask.Status == TaskStatus.RanToCompletion);
            Assert.IsNull(flowTask.Exception);
        }
    }
}
