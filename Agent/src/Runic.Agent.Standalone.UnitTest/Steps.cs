using FluentAssertions;
using Runic.Agent.Standalone.Test.TestUtility;
using Runic.Cucumber;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone.Test
{
    public class Steps
    {
        #region TestObjects
        Flow TestFlow => new Flow()
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
                                FunctionName = "Inputs",
                                Parameters = new Dictionary<string, Type>()
                                {
                                    { "input1", typeof(string) },
                                    { "4", typeof(int) }
                                }
                            }
                        }
                    }
        };
        #endregion

        private TestEnvironment _environment { get; set; }

        [Given(@"I have a test environment for a '(.*?)' flow")]
        public void SetupTestEnvironment(string patternType = "constant")
        {
            _environment = new TestEnvironment().WithStandardTypes();

            var mockAgentSettings = _environment.AgentSettings.MockObject;
            mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(3);
            mockAgentSettings.Setup(s => s.FlowPatternExecutionId).Returns("test_execution_id");
            mockAgentSettings.Setup(s => s.FlowThreadPatternName).Returns(patternType);
            mockAgentSettings.Setup(s => s.AgentFlowFilepath).Returns("test_path");

            switch (patternType.ToLowerInvariant()) {
                case "constant":
                    mockAgentSettings.Setup(s => s.FlowThreadCount).Returns(1);
                    break;
                case "graph":
                    break;
                case "gradual":
                    break;
            }
            _environment.AgentConfig
                       .MockObject.Setup(c => c.AgentSettings)
                       .Returns(_environment.AgentSettings.Instance);
            _environment.WithAgentConfig(_environment.AgentConfig.Instance);

            var testFlow = TestFlow;
            _environment.PluginManager
                       .MockObject
                       .Setup(p => p.GetClassType(testFlow.Steps[0].Function.AssemblyQualifiedClassName))
                       .Returns(typeof(FakeFunction));

            _environment.PluginManager
                      .MockObject
                      .Setup(p => p.GetPlugin(testFlow.Steps[0].Function.AssemblyName))
                      .Returns(GetType().GetTypeInfo().Assembly);

            _environment.FlowManager
                       .MockObject
                       .Setup(f => f.GetFlow("test_flow"))
                       .Returns(testFlow);

            _environment.FlowProvider
                       .MockObject
                       .Setup(p => p.GetFlow("test_path"))
                       .Returns(testFlow);

            _environment.StartApplication();
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
            FakeFunction.CreatedInstances.Any().Should().BeTrue("No FakeFunction instances found");
            FakeFunction.CreatedInstances.Max().Value.CallList.Any().Should().BeTrue("No methods invoked on FakeFunction instance");
        }
    }
}
