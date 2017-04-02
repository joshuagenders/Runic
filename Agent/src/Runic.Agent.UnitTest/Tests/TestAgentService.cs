using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Service;
using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.UnitTest.TestUtility;
using System;

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

        //todo add more

        [TestMethod]
        public async Task TestGradualFlowExecute()
        {
            var flow = new Flow()
            {
                Name = "FakeGradualFlow",
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
            };

            var agent = new AgentService(_world.PluginManager, 
                _world.MessagingService, 
                _world.FlowManager, 
                _world.Stats, 
                _world.DataService);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            var flowExecutionId = Guid.NewGuid().ToString("N");
            agent.ExecuteFlow(new GradualFlowExecutionRequest()
            {
                PatternExecutionId = flowExecutionId,
                Flow = flow,
                ThreadPattern = new GradualThreadModel()
                {
                    DurationSeconds = 4,
                    RampUpSeconds = 2,
                    ThreadCount= 4,
                    RampDownSeconds = 0,
                    StepIntervalSeconds = 1
                }
            }, cts.Token);
            Thread.Sleep(250);
            var runningFlows = agent.GetRunningFlows();
            var runningThreadPatterns = agent.GetRunningThreadPatterns();
            Assert.IsTrue(runningFlows.Contains(flowExecutionId),"running flow not found");
            Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");

            cts.Cancel();
            await agent.GetCompletionTask(flowExecutionId);
        }

        [TestMethod]
        public async Task TestGraphFlowExecute()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task TestConstantFlowExecute()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task TestMultipleDifferentFlowExecute()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task TestMultipleSameFlowExecute()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task TestHijackThreadPattern()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task TestHijackThreadLevel()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task TestHijackFlow()
        {
            throw new NotImplementedException();
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

            var agent = new AgentService(_world.PluginManager, _world.MessagingService, _world.FlowManager, _world.Stats, _world.DataService);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);

            await agent.SetThreadLevel(new SetThreadLevelRequest()
            {
                FlowName = "FakeFlow",
                FlowId = "MyFlow",
                ThreadLevel = 1
            }, cts.Token);

            var agentTask = agent.Run(cts.Token);

            Thread.Sleep(150);
            Assert.AreEqual(1, agent.GetThreadLevel("MyFlow"));

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
