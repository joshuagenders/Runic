using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Worker.Test.TestUtility;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Worker.Test.Tests
{
    [TestClass]
    public class MessagingTests
    {
        private TestEnvironment _testEnvironment { get; set; }
        private Flow _fakeFlow { get; set; }
        
        [TestInitialize]
        public void Init()
        {
            _fakeFlow = TestData.GetTestFlowSingleStepLooping;
        }

        [TestMethod]
        public async Task WorkerConstantFlowExecute_ExecutesFlow()
        {
            using (var scope = new TestStartup().BuildContainer().BeginLifetimeScope())
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(3000);
                _testEnvironment = (TestEnvironment)scope.Resolve<IApplication>();
                _testEnvironment.FlowManager.AddUpdateFlow(_fakeFlow);
                _testEnvironment.HandlerRegistry.RegisterMessageHandlers(cts.Token);
                try
                {
                    var flowExecutionId = Guid.NewGuid().ToString("N");
                    _testEnvironment.MessagingService.PublishMessage(new ConstantFlowExecutionRequest()
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
                    var runningFlows = _testEnvironment.ThreadManager.GetRunningFlows();
                    var runningThreadPatterns = _testEnvironment.PatternService.GetRunningThreadPatterns();
                    var runningPatternCount = _testEnvironment.PatternService.GetRunningThreadPatternCount();
                    var runningFlowCount = _testEnvironment.ThreadManager.GetRunningFlowCount();
                    Assert.IsTrue(runningPatternCount == 1, $"Running pattern count not 1, {runningPatternCount}");
                    Assert.IsTrue(runningFlowCount == 1, $"Running flow count not 1, {runningFlowCount}");
                    Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                    Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");

                    await _testEnvironment.PatternService.CancelAllPatternsAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // all g
                }
            }
        }

        [TestMethod]
        public async Task WorkerStartStopFlow_StartsAndStops()
        {
            using (var scope = new TestStartup().BuildContainer().BeginLifetimeScope())
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(3000);
                _testEnvironment = (TestEnvironment)scope.Resolve<IApplication>();
                _testEnvironment.HandlerRegistry.RegisterMessageHandlers(cts.Token);

                var flowExecutionId = Guid.NewGuid().ToString("N");
                _testEnvironment.MessagingService.PublishMessage(new ConstantFlowExecutionRequest()
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
                var runningFlows = _testEnvironment.ThreadManager.GetRunningFlows();
                var runningThreadPatterns = _testEnvironment.PatternService.GetRunningThreadPatterns();
                var runningPatternCount = _testEnvironment.PatternService.GetRunningThreadPatternCount();
                var runningFlowCount = _testEnvironment.ThreadManager.GetRunningFlowCount();
                Assert.IsTrue(runningPatternCount == 1, $"Running pattern count not 1, {runningPatternCount}");
                Assert.IsTrue(runningFlowCount == 1, $"Running flow count not 1, {runningFlowCount}");
                Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");
                try
                {
                    await _testEnvironment.PatternService.StopThreadPatternAsync(flowExecutionId, cts.Token);
                    _testEnvironment.ThreadManager.StopFlow(flowExecutionId);
                    runningPatternCount = _testEnvironment.PatternService.GetRunningThreadPatternCount();
                    runningFlowCount = _testEnvironment.ThreadManager.GetRunningFlowCount();
                    Assert.IsTrue(runningPatternCount == 0, $"Running pattern count not 0, {runningPatternCount}");
                    Assert.IsTrue(runningFlowCount == 0, $"Running flow count not 0, {runningFlowCount}");
                }
                catch (TaskCanceledException) { } //all g - todo handle better
                catch (AggregateException) { } //all g 
            }
        }

        [TestMethod]
        public async Task WhenGradualFlowExecutes_ExecutesFlow()
        {
            using (var scope = new TestStartup().BuildContainer().BeginLifetimeScope())
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(5000);
                _testEnvironment = (TestEnvironment)scope.Resolve<IApplication>();
                _testEnvironment.HandlerRegistry.RegisterMessageHandlers(cts.Token);

                try
                {
                    var flowExecutionId = Guid.NewGuid().ToString("N");
                    _testEnvironment.MessagingService.PublishMessage(
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
                    var runningFlows = _testEnvironment.ThreadManager.GetRunningFlows();
                    var runningThreadPatterns = _testEnvironment.PatternService.GetRunningThreadPatterns();
                    Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                    Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");
                    await _testEnvironment.PatternService.CancelAllPatternsAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // all g
                }
            }
        }

        [TestMethod]
        public async Task WhenGraphFlowExecutes_ExecutesFlow()
        {
            using (var scope = new TestStartup().BuildContainer().BeginLifetimeScope())
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(3000);
                _testEnvironment = (TestEnvironment)scope.Resolve<IApplication>();
                _testEnvironment.HandlerRegistry.RegisterMessageHandlers(cts.Token);

                try
                {
                    var flowExecutionId = Guid.NewGuid().ToString("N");
                    _testEnvironment.MessagingService.PublishMessage(new GraphFlowExecutionRequest()
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
                    var runningFlows = _testEnvironment.ThreadManager.GetRunningFlows();
                    var runningThreadPatterns = _testEnvironment.PatternService.GetRunningThreadPatterns();
                    var runningPatternCount = _testEnvironment.PatternService.GetRunningThreadPatternCount();
                    var runningFlowCount = _testEnvironment.ThreadManager.GetRunningFlowCount();
                    Assert.IsTrue(runningPatternCount == 1, $"Running pattern count not 1, {runningPatternCount}");
                    Assert.IsTrue(runningFlowCount == 1, $"Running flow count not 1, {runningFlowCount}");
                    Assert.IsTrue(runningFlows.Contains(flowExecutionId), "running flow not found");
                    Assert.IsTrue(runningThreadPatterns.Contains(flowExecutionId), "running thread pattern not found");

                    await _testEnvironment.PatternService.CancelAllPatternsAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // all g
                }
            }
        }
        
        [TestMethod]
        public async Task WhenFlowStartMessageSent_FlowIsExecuted()
        {
            using (var scope = new TestStartup().BuildContainer().BeginLifetimeScope())
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(6200);
                _testEnvironment = (TestEnvironment)scope.Resolve<IApplication>();
                _testEnvironment.HandlerRegistry.RegisterMessageHandlers(cts.Token);

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

                _testEnvironment.MessagingService.PublishMessage(startRequest);
                Thread.Sleep(200);
                Assert.AreEqual(1, _testEnvironment.ThreadManager.GetRunningFlowCount(), "Running flows was not 1");
                Assert.AreEqual(1, _testEnvironment.PatternService.GetRunningThreadPatternCount(), "Running thread patterns was not 1");

                await _testEnvironment.PatternService.CancelAllPatternsAsync(cts.Token);
            }
        }
    }
}
