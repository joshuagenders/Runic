using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Runic.Agent.UnitTest.TestUtility;
//using Runic.Framework.Models;
using System;
using System.Threading;

namespace Runic.Agent.Worker.UnitTest.Tests
{
    [TestClass]
    public class TestMessaging
    {

        /*
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
            var runningFlows = _testEnvironment.App.PatternService.GetRunningFlows.ToList();
            var runningThreadPatterns = _testEnvironment.App.PatternService.GetRunningThreadPatterns.ToList();
            var runningPatternCount = _testEnvironment.App.PatternService.GetRunningThreadPatternCount;
            var runningFlowCount = _testEnvironment.App.PatternService.GetRunningFlowCount;
            Assert.IsTrue(runningPatternCount == 1, $"Running pattern count not 1, {runningPatternCount}");
            Assert.IsTrue(runningFlowCount == 1, $"Running flow count not 1, {runningFlowCount}");
            Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
            Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");
            try
            {
                _testEnvironment.App.PatternService.StopThreadPattern(flowExecutionId);
                _testEnvironment.App.PatternService.StopFlow(flowExecutionId);
                runningPatternCount = _testEnvironment.App.PatternService.GetRunningThreadPatternCount;
                runningFlowCount = _testEnvironment.App.PatternService.GetRunningFlowCount;
                Assert.IsTrue(runningPatternCount == 0, $"Running pattern count not 0, {runningPatternCount}");
                Assert.IsTrue(runningFlowCount == 0, $"Running flow count not 0, {runningFlowCount}");
            }
            catch (TaskCanceledException){ } //all g - todo handle better
            catch (AggregateException){ } //all g 
        }
         */

        /*
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
                var runningFlows = _testEnvironment.App.PatternService.GetRunningFlows;
                var runningThreadPatterns = _testEnvironment.App.PatternService.GetRunningThreadPatterns;
                Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");
                _testEnvironment.App.PatternService.SafeCancelAll(cts.Token);
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
                var runningFlows = _testEnvironment.App.PatternService.GetRunningFlows.ToList();
                var runningThreadPatterns = _testEnvironment.App.PatternService.GetRunningThreadPatterns.ToList();
                var runningPatternCount = _testEnvironment.App.PatternService.GetRunningThreadPatternCount;
                var runningFlowCount = _testEnvironment.App.PatternService.GetRunningFlowCount;
                Assert.IsTrue(runningPatternCount == 1, $"Running pattern count not 1, {runningPatternCount}");
                Assert.IsTrue(runningFlowCount == 1, $"Running flow count not 1, {runningFlowCount}");
                Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");

                _testEnvironment.App.PatternService.SafeCancelAll(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // all g
            }
        }
         */

        /*
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
                var runningFlows = _testEnvironment.App.PatternService.GetRunningFlows;
                var runningThreadPatterns = _testEnvironment.App.PatternService.GetRunningThreadPatterns;
                Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");
                _testEnvironment.App.PatternService.SafeCancelAll(cts.Token);
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
                var runningFlows = _testEnvironment.App.PatternService.GetRunningFlows.ToList();
                var runningThreadPatterns = _testEnvironment.App.PatternService.GetRunningThreadPatterns.ToList();
                var runningPatternCount = _testEnvironment.App.PatternService.GetRunningThreadPatternCount;
                var runningFlowCount = _testEnvironment.App.PatternService.GetRunningFlowCount;
                Assert.IsTrue(runningPatternCount == 1, $"Running pattern count not 1, {runningPatternCount}");
                Assert.IsTrue(runningFlowCount == 1, $"Running flow count not 1, {runningFlowCount}");
                Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");

                _testEnvironment.App.PatternService.SafeCancelAll(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // all g
            }
        }
         */

        /*
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
                var runningFlows = _testEnvironment.App.PatternService.GetRunningFlows.ToList();
                var runningThreadPatterns = _testEnvironment.App.PatternService.GetRunningThreadPatterns.ToList();
                var runningPatternCount = _testEnvironment.App.PatternService.GetRunningThreadPatternCount;
                var runningFlowCount = _testEnvironment.App.PatternService.GetRunningFlowCount;
                Assert.IsTrue(runningPatternCount == 1, $"Running pattern count not 1, {runningPatternCount}");
                Assert.IsTrue(runningFlowCount == 1, $"Running flow count not 1, {runningFlowCount}");
                Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");

                _testEnvironment.App.PatternService.SafeCancelAll(cts.Token);
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

         */

        /*
         * private TestEnvironment _testEnvironment { get; set; }
        
        [TestInitialize]
        public void Init()
        {
            _testEnvironment = new TestEnvironment();
        }
        
        [TestMethod]
        public void TestFlowStartMessage()
        {
            var flowId = Guid.NewGuid().ToString("N");
            var startRequest = new ConstantFlowExecutionRequest()
            {
                Flow = TestData.GetTestFlowSingleStepLooping,
                PatternExecutionId = flowId,
                ThreadPattern = new ConstantThreadModel()
                {
                    DurationSeconds = 3,
                    ThreadCount = 2
                }
            };
        
            _testEnvironment.App.MessagingService.PublishMessage(startRequest);
            Thread.Sleep(200);
            Assert.AreEqual(1, _testEnvironment.App.PatternService.GetRunningFlowCount, "Running flows was not 1");
            Assert.AreEqual(1, _testEnvironment.App.PatternService.GetRunningThreadPatternCount, "Running thread patterns was not 1");
            
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            _testEnvironment.App.PatternService.SafeCancelAll(cts.Token);
        }
        */
    }
}
