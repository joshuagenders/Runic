using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.UnitTest.TestUtility;
using System;
using System.Linq;
using Runic.Agent.ThreadManagement;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestThreadOrchestrator
    {
        private TestEnvironment _testEnvironment { get; set; }
        private Flow _fakeFlow { get; set; }

        [TestInitialize]
        public void Init()
        {
            _testEnvironment = new TestEnvironment();
        
            _fakeFlow = TestData.GetTestFlowSingleStepLooping;
        }
        
        [TestMethod]
        public void TestGradualFlowExecute()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            try
            {
                var flowExecutionId = Guid.NewGuid().ToString("N");
                _testEnvironment.App.MessagingService.PublishMessage(
                    new GradualFlowExecutionRequest()
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
                    });

                Thread.Sleep(1250);
                var runningFlows = _testEnvironment.App.ThreadOrchestrator.GetRunningFlows;
                var runningThreadPatterns = _testEnvironment.App.ThreadOrchestrator.GetRunningThreadPatterns;
                Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");
                _testEnvironment.App.ThreadOrchestrator.SafeCancelAll(cts.Token);
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
                _testEnvironment.App.MessagingService.PublishMessage(new GraphFlowExecutionRequest()
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
                });

                Thread.Sleep(250);
                var runningFlows = _testEnvironment.App.ThreadOrchestrator.GetRunningFlows.ToList();
                var runningThreadPatterns = _testEnvironment.App.ThreadOrchestrator.GetRunningThreadPatterns.ToList();
                var runningPatternCount = _testEnvironment.App.ThreadOrchestrator.GetRunningThreadPatternCount;
                var runningFlowCount = _testEnvironment.App.ThreadOrchestrator.GetRunningFlowCount;
                Assert.IsTrue(runningPatternCount == 1, $"Running pattern count not 1, {runningPatternCount}");
                Assert.IsTrue(runningFlowCount == 1, $"Running flow count not 1, {runningFlowCount}");
                Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");

                _testEnvironment.App.ThreadOrchestrator.SafeCancelAll(cts.Token);
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
                _testEnvironment.App.MessagingService.PublishMessage(new ConstantFlowExecutionRequest()
                {
                    PatternExecutionId = flowExecutionId,
                    Flow = _fakeFlow,
                    ThreadPattern = new ConstantThreadModel()
                    {
                        DurationSeconds = 2,
                        ThreadCount = 3
                    }
                });

                Thread.Sleep(1150);
                var runningFlows = _testEnvironment.App.ThreadOrchestrator.GetRunningFlows.ToList();
                var runningThreadPatterns = _testEnvironment.App.ThreadOrchestrator.GetRunningThreadPatterns.ToList();
                var runningPatternCount = _testEnvironment.App.ThreadOrchestrator.GetRunningThreadPatternCount;
                var runningFlowCount = _testEnvironment.App.ThreadOrchestrator.GetRunningFlowCount;
                Assert.IsTrue(runningPatternCount == 1, $"Running pattern count not 1, {runningPatternCount}");
                Assert.IsTrue(runningFlowCount == 1, $"Running flow count not 1, {runningFlowCount}");
                Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");

                _testEnvironment.App.ThreadOrchestrator.SafeCancelAll(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // all g
            }
        }

        //[TestMethod]
        public void TestMultipleDifferentFlowExecute()
        {
            throw new NotImplementedException();
        }

        //[TestMethod]
        public void TestMultipleSameFlowExecute()
        {
            throw new NotImplementedException();
        }

        //[TestMethod]
        public void TestHijackThreadPattern()
        {
            throw new NotImplementedException();
        }

        //[TestMethod]
        public void TestHijackThreadLevel()
        {
            throw new NotImplementedException();
        }

        //[TestMethod]
        public void TestHijackFlow()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestStartStopFlow()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(3000);            
            var flowExecutionId = Guid.NewGuid().ToString("N");
            _testEnvironment.App.MessagingService.PublishMessage(new ConstantFlowExecutionRequest()
            {
                PatternExecutionId = flowExecutionId,
                Flow = _fakeFlow,
                ThreadPattern = new ConstantThreadModel()
                {
                    DurationSeconds = 2,
                    ThreadCount = 3
                }
            });

            Thread.Sleep(1150);
            var runningFlows = _testEnvironment.App.ThreadOrchestrator.GetRunningFlows.ToList();
            var runningThreadPatterns = _testEnvironment.App.ThreadOrchestrator.GetRunningThreadPatterns.ToList();
            var runningPatternCount = _testEnvironment.App.ThreadOrchestrator.GetRunningThreadPatternCount;
            var runningFlowCount = _testEnvironment.App.ThreadOrchestrator.GetRunningFlowCount;
            Assert.IsTrue(runningPatternCount == 1, $"Running pattern count not 1, {runningPatternCount}");
            Assert.IsTrue(runningFlowCount == 1, $"Running flow count not 1, {runningFlowCount}");
            Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
            Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");
            try
            {
                _testEnvironment.App.ThreadOrchestrator.StopThreadPattern(flowExecutionId);
                _testEnvironment.App.ThreadOrchestrator.StopFlow(flowExecutionId);
                runningPatternCount = _testEnvironment.App.ThreadOrchestrator.GetRunningThreadPatternCount;
                runningFlowCount = _testEnvironment.App.ThreadOrchestrator.GetRunningFlowCount;
                Assert.IsTrue(runningPatternCount == 0, $"Running pattern count not 0, {runningPatternCount}");
                Assert.IsTrue(runningFlowCount == 0, $"Running flow count not 0, {runningFlowCount}");
            }
            catch (TaskCanceledException){ } //all g - todo handle better
            catch (AggregateException){ } //all g 
        }

        [TestMethod]
        public async Task TestSetThreadLevel()
        {
            _testEnvironment.App.FlowManager.AddUpdateFlow(
                TestData.GetTestFlowSingleStepLooping);

            var agent = new ThreadOrchestrator(
                _testEnvironment.App.PluginManager,
                _testEnvironment.App.FlowManager,
                _testEnvironment.App.Stats,
                _testEnvironment.App.DataService);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);

            await agent.SetThreadLevelAsync(new SetThreadLevelRequest()
            {
                FlowName = "Test",
                FlowId = "MyFlow",
                ThreadLevel = 1
            }, cts.Token);

            Thread.Sleep(150);
            Assert.AreEqual(1, agent.GetThreadLevel("MyFlow"));

            try
            {
                cts.Cancel();
            }
            catch (TaskCanceledException)
            {
                
            }
            catch (OperationCanceledException)
            {

            }
        }
    }
}
