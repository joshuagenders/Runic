using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.UnitTest.TestUtility;
using Runic.Framework.Models;
using System;
using System.Threading;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestMessaging
    {
        private TestEnvironment _testEnvironment { get; set; }

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
            Assert.AreEqual(1, _testEnvironment.App.ThreadOrchestrator.GetRunningFlowCount, "Running flows was not 1");
            Assert.AreEqual(1, _testEnvironment.App.ThreadOrchestrator.GetRunningThreadPatternCount, "Running thread patterns was not 1");
            
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            _testEnvironment.App.ThreadOrchestrator.SafeCancelAll(cts.Token);
        }
    }
}
