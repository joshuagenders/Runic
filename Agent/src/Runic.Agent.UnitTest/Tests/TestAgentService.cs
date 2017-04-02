using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Service;
using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.UnitTest.TestUtility;
using System;
using System.Linq;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestAgentService
    {
        private AgentWorld _world { get; set; }
        private Flow _fakeFlow { get; set; }

        [TestInitialize]
        public void Init()
        {
            _world = new AgentWorld();

            _fakeFlow = new Flow()
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
            };
        }
        
        [TestMethod]
        public void TestGradualFlowExecute()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            try
            {
                var flowExecutionId = Guid.NewGuid().ToString("N");
                _world.Agent.ExecuteFlow(new GradualFlowExecutionRequest()
                {
                    PatternExecutionId = flowExecutionId,
                    Flow = _fakeFlow,
                    ThreadPattern = new GradualThreadModel()
                    {
                        DurationSeconds = 4,
                        RampUpSeconds = 2,
                        ThreadCount = 4,
                        RampDownSeconds = 1,
                        StepIntervalSeconds = 1
                    }
                }, cts.Token);
                Thread.Sleep(1250);
                var runningFlows = _world.Agent.GetRunningFlows();
                var runningThreadPatterns = _world.Agent.GetRunningThreadPatterns();
                Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");
                _world.Agent.SafeCancelAll(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // all g
            }
        }

        [TestMethod]
        public void TestGraphFlowExecute()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(3000);
            try { 
                var flowExecutionId = Guid.NewGuid().ToString("N");
                _world.Agent.ExecuteFlow(new GraphFlowExecutionRequest()
                {
                    PatternExecutionId = flowExecutionId,
                    Flow = _fakeFlow,
                    ThreadPattern = new GraphThreadModel()
                    {
                        DurationSeconds = 2,
                        Points = new List<Point>()
                        {
                            new Point(){ threadLevel = 2, unitsFromStart = 0 },
                            new Point() { threadLevel = 0, unitsFromStart = 10}
                        }
                    }
                }, cts.Token);
                Thread.Sleep(250);
                var runningFlows = _world.Agent.GetRunningFlows().ToList();
                var runningThreadPatterns = _world.Agent.GetRunningThreadPatterns().ToList();
                var runningPatternCount = _world.Agent.GetRunningThreadPatternCount();
                var runningFlowCount = _world.Agent.GetRunningFlowCount();
                Assert.IsTrue(runningPatternCount == 1, $"Running pattern count not 1, {runningPatternCount}");
                Assert.IsTrue(runningFlowCount == 1, $"Running flow count not 1, {runningFlowCount}");
                Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");

                _world.Agent.SafeCancelAll(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // all g
            }
        }

        [TestMethod]
        public void TestConstantFlowExecute()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(3000);
            try {
                var flowExecutionId = Guid.NewGuid().ToString("N");
                _world.Agent.ExecuteFlow(new ConstantFlowExecutionRequest()
                {
                    PatternExecutionId = flowExecutionId,
                    Flow = _fakeFlow,
                    ThreadPattern = new ConstantThreadModel()
                    {
                        DurationSeconds = 2,
                        ThreadCount = 3
                    }
                }, cts.Token);
                Thread.Sleep(1150);
                var runningFlows = _world.Agent.GetRunningFlows().ToList();
                var runningThreadPatterns = _world.Agent.GetRunningThreadPatterns().ToList();
                var runningPatternCount = _world.Agent.GetRunningThreadPatternCount();
                var runningFlowCount = _world.Agent.GetRunningFlowCount();
                Assert.IsTrue(runningPatternCount == 1, $"Running pattern count not 1, {runningPatternCount}");
                Assert.IsTrue(runningFlowCount == 1, $"Running flow count not 1, {runningFlowCount}");
                Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");

                _world.Agent.SafeCancelAll(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // all g
            }
        }

        [TestMethod]
        public void TestMultipleDifferentFlowExecute()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestMultipleSameFlowExecute()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestHijackThreadPattern()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestHijackThreadLevel()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestHijackFlow()
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
