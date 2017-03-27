using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.FlowManagement;
using Runic.Agent.Service;
using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestAgentService
    {
        private AgentWorld _world { get; set; }

        [TestInitialize]
        public void Init()
        {
            _world = new AgentWorld();
        }

        [TestMethod]
        public async Task TestSetThreadLevel()
        {
            var flows = new Flows();
            
            flows.AddUpdateFlow(
                new Flow()
                {
                    Name = "FakeFlow",
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
                            NextStepOnSuccess = "Step1"
                        }
                    }
                });

            var agent = new AgentService(_world.PluginManager, flows);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);

            await agent.SetThreadLevel(new SetThreadLevelRequest()
            {
                FlowName = "FakeFlow",
                ThreadLevel = 1
            }, cts.Token);

            var agentTask = agent.Run(_world.MessagingService, cts.Token);

            Thread.Sleep(150);
            Assert.AreEqual(1, agent.GetThreadLevel("FakeFlow"));

            try
            {
                cts.Cancel();
                await agentTask;
            }
            catch (TaskCanceledException)
            {
                
            }
        }
    }
}
