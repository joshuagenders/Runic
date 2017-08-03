using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Test.TestUtility;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Runic.Agent.Standalone.Test
{
    [TestClass]
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
                                AssemblyName = "myassembly",
                                AssemblyQualifiedClassName = "myclassname",
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

        [TestMethod]
        public void TestConstantPatternExecution()
        {
            var mockAgentSettings = new Mock<IAgentSettings>();
            mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(3);
            mockAgentSettings.Setup(s => s.FlowPatternExecutionId).Returns("test_execution_id");
            mockAgentSettings.Setup(s => s.FlowThreadCount).Returns(1);
            mockAgentSettings.Setup(s => s.FlowThreadPatternName).Returns("TestConstantThreadPattern");
            
            var environment = new TestEnvironment().WithAgentSettings(mockAgentSettings.Object)
                                                   .WithType<AgentConfig, IAgentConfig>()
                                                   .WithType<StatsdSettings, IStatsdSettings>()
                                                   .WithStandardTypes();

            var testFlow = TestFlow;
            environment.PluginManager
                       .MockObject
                       .Setup(p => p.GetClassType(testFlow.Steps[0].Function.FunctionName))
                       .Returns(typeof(FakeFunction));

            environment.FlowManager
                       .MockObject
                       .Setup(f => f.GetFlow("test_flow"))
                       .Returns(testFlow);
            
            environment.StartApplication();
            var cts = new CancellationTokenSource();
            var appTask = environment.Application.RunApplicationAsync(cts.Token);
            environment.ThreadManager.Instance.FlowExists("test_flow").Should().BeTrue();
        }

        [TestMethod]
        public void TestGradualPatternExecution()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestGraphPatternExecution()
        {
            throw new NotImplementedException();
        }
    }
}
