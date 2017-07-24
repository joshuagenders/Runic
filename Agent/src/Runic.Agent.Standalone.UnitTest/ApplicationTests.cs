using FluentAssertions;
using Moq;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Test.TestUtility;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;

namespace Runic.Agent.Standalone.Test
{
    public class ApplicationTests
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
                            Function = new FunctionInformation()
                            {
                                AssemblyName = "",
                                AssemblyQualifiedClassName = "",
                                FunctionName = "",
                                Parameters = new Dictionary<string, Type>()
                                {

                                }
                            }
                        }
                    }
        };
        #endregion

        public void TestConstantPatternExecution()
        {
            var mockAgentSettings = new Mock<IAgentSettings>();
            mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(3);
            mockAgentSettings.Setup(s => s.FlowPatternExecutionId).Returns("test_execution_id");
            mockAgentSettings.Setup(s => s.FlowThreadCount).Returns(1);
            mockAgentSettings.Setup(s => s.FlowThreadPatternName).Returns("TestConstantThreadPattern");

            var environment = new TestEnvironment().WithAgentSettings(mockAgentSettings.Object)
                                                   .WithType<AgentConfig, IAgentConfig>()
                                                   .StartApplication();
            environment.FlowManager
                       .MockObject
                       .Setup(f => f.GetFlow("test_flow"))
                       .Returns(TestFlow);

            environment.StartApplication();
            environment.ThreadManager.Instance.FlowExists("test_flow").Should().BeTrue();
        }

        public void TestGradualPatternExecution()
        {
            var mockAgentSettings = new Mock<IAgentSettings>();
            mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(4);
            mockAgentSettings.Setup(s => s.FlowPatternExecutionId).Returns("test_execution_id");
            mockAgentSettings.Setup(s => s.FlowRampDownSeconds).Returns(1);
            mockAgentSettings.Setup(s => s.FlowRampUpSeconds).Returns(1);
            mockAgentSettings.Setup(s => s.FlowStepIntervalSeconds).Returns(1);
            mockAgentSettings.Setup(s => s.FlowThreadCount).Returns(1);
            mockAgentSettings.Setup(s => s.FlowThreadPatternName).Returns("TestGradualThreadPatterm");

            var environment = new TestEnvironment().WithAgentSettings(mockAgentSettings.Object)
                                                   .WithType<AgentConfig, IAgentConfig>()
                                                   .StartApplication();
            environment.FlowManager
                       .MockObject
                       .Setup(f => f.GetFlow("test_flow"))
                       .Returns(TestFlow);

        }

        public void TestGraphPatternExecution()
        {
            var mockAgentSettings = new Mock<IAgentSettings>();
            mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(4);
            mockAgentSettings.Setup(s => s.FlowPatternExecutionId).Returns("test_execution_id");
            mockAgentSettings.Setup(s => s.FlowPoints).Returns(new string[] { "0.1", "2.2", "4.0" });
            mockAgentSettings.Setup(s => s.FlowStepIntervalSeconds).Returns(1);
            mockAgentSettings.Setup(s => s.FlowThreadPatternName).Returns("TestGraphThreadPattern");

            var environment = new TestEnvironment().WithAgentSettings(mockAgentSettings.Object)
                                                   .WithType<AgentConfig, IAgentConfig>()
                                                   .StartApplication();
            environment.FlowManager
                       .MockObject
                       .Setup(f => f.GetFlow("test_flow"))
                       .Returns(TestFlow);

        }
    }
}
