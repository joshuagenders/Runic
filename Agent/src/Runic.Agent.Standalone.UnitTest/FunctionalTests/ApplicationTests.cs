using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.TestHarness.StepController;
using Runic.Agent.ExampleTest.Functions;
using Runic.Agent.Standalone.Test.TestUtility;
using Runic.Agent.Framework.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone.Test.FunctionalTests
{
    [TestClass]
    public class ApplicationTests
    {
        private StandaloneTestEnvironment _environment { get; set; }

        [TestInitialize]
        public void Init()
        {
            _environment = new StandaloneTestEnvironment();
        }
        
        [TestCategory("FunctionalTest")]
        [TestMethod]
        public async Task WhenStepDistributionIsEven_ThenStepExecutionIsEven()
        {
            var flow = TestFlows.EvenStepDistributionFlow;
            await RunApplication(flow, "constant");

            //Todo assert distribution even

            AssertLogsCreatedWithoutErrors();
            AssertSuccessfulTestResult(flow);

            //TODO assertions on:
            //step distribution
        }

        [TestCategory("FunctionalTest")]
        [TestMethod]
        public async Task WhenGraphPatternIsExecuted_ThenThreadLevelsAreSet()
        {
            var flow = TestFlows.SystemTestFlow;
            await RunApplication(flow, "graph");
            //todo assert thread levels

            AssertLogsCreatedWithoutErrors();
            AssertSuccessfulTestResult(flow);
        }

        [TestCategory("FunctionalTest")]
        [TestMethod]
        public async Task WhenStepRepeats_ThenStepIsRepeated()
        {
            var flow = TestFlows.StepRepeatFlow;
            await RunApplication(flow, "constant");

            //todo assert step repeated

            AssertLogsCreatedWithoutErrors();
            AssertSuccessfulTestResult(flow);
        }

        [TestCategory("FunctionalTest")]
        [TestMethod]
        public async Task WhenStringReturn_ThenFlowFollowsReturnStringValues()
        {
            var flow = TestFlows.StringReturnFlow;
            await RunApplication(flow, "constant");

            AssertLogsCreatedWithoutErrors();
            AssertSuccessfulTestResult(flow); ;
        }

        [TestCategory("FunctionalTest")]
        [TestMethod]
        public async Task WhenFunctionTestIsExecuted_MultipleStepsAreExecutedAndSuccessful()
        {
            var flow = TestFlows.SystemTestFlow;
            await RunApplication(flow, "constant");

            AssertLogsCreatedWithoutErrors();
            AssertSuccessfulTestResult(flow);
        }

        private void AssertLogsCreatedWithoutErrors()
        {
            _environment.EventHandler.MockObject.Verify(l => l.Debug(It.IsAny<string>(), null));
            _environment.EventHandler.MockObject.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        private void AssertSuccessfulTestResult(Flow flow)
        {
            _environment.EventHandler.MockObject.Verify(r => r.OnFlowStart(flow));
            //_environment.EventHandler.MockObject.Verify(r => r.OnTestResult(It.Is<Result>(f => f.Success)));
            _environment.EventHandler.MockObject.Verify(r => r.OnThreadChange(flow, _environment.AgentConfig.Instance.AgentSettings.FlowThreadCount));
          
            _environment.EventHandler.MockObject.Verify(r => r.OnThreadChange(flow, 0));
            _environment.EventHandler.MockObject.Verify(r => r.OnFlowComplete(flow));
        }

        private async Task RunApplication(Flow flow, string patternType)
        {
            SetupTestEnvironment(flow, patternType);

            var startTime = DateTime.Now;
            _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1200);

            _environment.StartApplication();
            var appTask = _environment.Application.RunApplicationAsync(cts.Token);
            
            for (int i = 0; i < 60; i++)
            {
                _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime.AddSeconds(i));
                Thread.Sleep(12);
            }

            _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime.AddMinutes(2));
            await appTask;
        }

        public void SetupTestEnvironment(Flow flow, string patternType = "constant")
        {
            var mockAgentSettings = _environment.AgentSettings.MockObject;
            mockAgentSettings.Setup(s => s.FlowPatternExecutionId).Returns("test_execution_id");
            mockAgentSettings.Setup(s => s.FlowThreadPatternName).Returns(patternType);
            mockAgentSettings.Setup(s => s.AgentFlowFilepath).Returns("test_path");
            mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(50);

            switch (patternType.ToLowerInvariant())
            {
                case "constant":
                    mockAgentSettings.Setup(s => s.FlowThreadCount).Returns(1);
                    mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(50);
                    break;
                case "graph":
                    mockAgentSettings.Setup(s => s.FlowPoints).Returns(new string[] { "0.1", "3.0" });
                    break;
                case "gradual":
                    mockAgentSettings.Setup(s => s.FlowRampUpSeconds).Returns(5);
                    mockAgentSettings.Setup(s => s.FlowRampDownSeconds).Returns(5);
                    mockAgentSettings.Setup(s => s.FlowThreadCount).Returns(1);
                    mockAgentSettings.Setup(s => s.FlowStepIntervalSeconds).Returns(1);
                    break;
            }
            _environment.AgentConfig
                        .MockObject
                        .Setup(c => c.AgentSettings)
                        .Returns(_environment.AgentSettings.Instance);

            _environment.FlowProvider
                        .MockObject
                        .Setup(p => p.GetFlow("test_path"))
                        .Returns(flow);
        }
    }
}
