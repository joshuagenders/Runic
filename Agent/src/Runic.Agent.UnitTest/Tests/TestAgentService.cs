using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Service;
using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.UnitTest.TestUtility;

namespace Runic.Agent.UnitTest.Tests
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
            _world.FlowManager.AddUpdateFlow(
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

            var agent = new AgentService(_world.PluginManager, _world.MessagingService, _world.FlowManager, _world.Stats);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);

            await agent.SetThreadLevel(new SetThreadLevelRequest()
            {
                FlowName = "FakeFlow",
                ThreadLevel = 1
            }, cts.Token);

            var agentTask = agent.Run(cts.Token);

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
