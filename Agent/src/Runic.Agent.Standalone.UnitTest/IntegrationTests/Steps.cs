using Moq;
using RestMockCore;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.CucumberHarness;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.Services.Interfaces;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Providers;
using Runic.Agent.Standalone.Test.TestUtility;
using Runic.Cucumber;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone.Test.IntegrationTests
{
    public class Steps
    {
        #region TestObjects
        Flow FunctionFlow => new Flow()
        {
            Name = "test_flow",
            StepDelayMilliseconds = 10,
            Steps =
                new List<Step>()
                    {
                        new Step()
                        {
                            StepName = "step1",
                            GetNextStepFromFunctionResult = false,
                            Function = new FunctionInformation()
                            {
                                AssemblyName = "Runic.Agent.Standalone.Test",
                                AssemblyQualifiedClassName = "FakeFunction",
                                FunctionName = "Login"
                            }
                        }
                    }
        };
        Flow CucumberFlow => new Flow()
        {
            Name = "test_flow",
            StepDelayMilliseconds = 10,
            Steps =
                new List<Step>()
                    {
                        new Step()
                        {
                            StepName = "step1",
                            GetNextStepFromFunctionResult = false,
                            Cucumber = new CucmberInformation()
                            {
                                AssemblyName = "Runic.Agent.Standalone.Test",
                                Document =
                                @"Feature: MyExample
                                  Scenario: MyScenario
                                   Given I have a given ""method""
                                   When I have a when ""wherever""
                                   Then I have a then ""whomever"""
                            }
                        }
                    }
        };
        #endregion

        private TestEnvironment _environment { get; set; }
        private FakeFunction _fakeFunction { get; set; }

        [Given("I have a test server")]
        private void GivenIHaveATestServer()
        {
            var server = new HttpServer();
            server.Config.Get("/").Send("OK");
            server.Run();
        }

        [Given(@"I have a test environment for a '(.*?)' flow")]
        public void SetupTestEnvironment(string patternType = "constant")
        {
            _environment = new TestEnvironmentBuilder().WithStandardTypes()
                                                       .With<IDatetimeService>(new DateTimeService());

            var mockAgentSettings = _environment.GetMock<IAgentSettings>();
            mockAgentSettings.Setup(s => s.FlowPatternExecutionId).Returns("test_execution_id");
            mockAgentSettings.Setup(s => s.FlowThreadPatternName).Returns(patternType);
            mockAgentSettings.Setup(s => s.AgentFlowFilepath).Returns("test_path");
            mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(3);
            
            switch (patternType.ToLowerInvariant()) {
                case "constant":
                    mockAgentSettings.Setup(s => s.FlowThreadCount).Returns(1);
                    mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(3);
                    break;
                case "graph":
                    mockAgentSettings.Setup(s => s.FlowPoints).Returns(new string[] { "0.1", "3.0" });
                    break;
                case "gradual":
                    mockAgentSettings.Setup(s => s.FlowRampUpSeconds).Returns(0);
                    mockAgentSettings.Setup(s => s.FlowRampDownSeconds).Returns(0);
                    mockAgentSettings.Setup(s => s.FlowThreadCount).Returns(1);
                    mockAgentSettings.Setup(s => s.FlowStepIntervalSeconds).Returns(1);
                    break;
            }
            _environment.GetMock<IAgentConfig>()
                        .Setup(c => c.AgentSettings)
                        .Returns(_environment.Get<IAgentSettings>());
            _environment.With(_environment.Get<IAgentConfig>());
        }

        public void RegisterTestFlow(Flow testFlow)
        {
            var assemblyName = testFlow.Steps[0].Function?.AssemblyName ?? testFlow.Steps[0].Cucumber.AssemblyName;
            _environment.GetMock<IPluginManager>()
                        .Setup(p => p.GetPlugin(assemblyName))
                        .Returns(GetType().GetTypeInfo().Assembly);

            _environment.GetMock<IFlowManager>()
                        .Setup(f => f.GetFlow("test_flow"))
                        .Returns(testFlow);

            _environment.GetMock<IFlowProvider>()
                        .Setup(p => p.GetFlow("test_path"))
                        .Returns(testFlow);
        }

        [Given("I start the application")]
        public void StartApplication()
        {
            _environment.StartApplication();
        }

        [Given("I have a cucumber flow")]
        public void IHaveACucumberTest()
        {
            RegisterTestFlow(CucumberFlow);
        }

        [Given("I have a function flow")]
        public void IHaveAFunctionFlow()
        {
            RegisterTestFlow(FunctionFlow);

            _fakeFunction = new FakeFunction();
            _environment.GetMock<IPluginManager>()
                        .Setup(p => p.GetInstance(FunctionFlow.Steps[0].Function.AssemblyQualifiedClassName))
                        .Returns(_fakeFunction);
        }

        [When(@"I start the test")]
        public async Task WhenIStartTheTest()
        {
            var cts = new CancellationTokenSource();
            var appTask = _environment.Application.RunApplicationAsync(cts.Token);
            await appTask;
        }

        [Then(@"The fake function is invoked")]
        public void TheFakeFunctionIsInvoked()
        {
            var handler = _environment.GetMock<ITestResultHandler>();
            handler.Verify(h => h.OnFunctionComplete(It.Is<FunctionResult>(r => r.Success)));
        }

        [Then(@"The fake cucumber test is invoked")]
        public void TheFakeCucumberTestIsInvoked()
        {
            var handler = _environment.GetMock<ITestResultHandler>();
            handler.Verify(h => h.OnCucumberComplete(It.Is<CucumberResult>(r => r.Success)));
        }
    }
}
