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
        private PluginManager _pluginManager { get; set; }

        [TestInitialize]
        public void Init()
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
            _pluginManager = new PluginManager();
            _pluginManager.RegisterProvider(container.Resolve<IPluginProvider>());
            _pluginManager.LoadPlugin("Runic.ExampleTest");
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

            var flowInit = new FlowInitialiser(_pluginManager);
            flowInit.InitialiseFlow(flow);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(500);
            var flowExecutor = new FlowExecutor(flow, _pluginManager);
            var flowTask = flowExecutor.ExecuteFlow(cts.Token);
            await flowTask;
            Assert.IsTrue(flowTask.Status == TaskStatus.RanToCompletion);
            Assert.IsNull(flowTask.Exception);
        }
    }
}
