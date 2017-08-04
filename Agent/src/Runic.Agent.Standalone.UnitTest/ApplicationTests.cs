using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Standalone.Test.TestUtility;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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

        [TestMethod]
        public async Task TestConstantPatternExecution()
        {
            var environment = new TestEnvironment().WithStandardTypes();
            
            var mockAgentSettings = environment.AgentSettings.MockObject;
            mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(3);
            mockAgentSettings.Setup(s => s.FlowPatternExecutionId).Returns("test_execution_id");
            mockAgentSettings.Setup(s => s.FlowThreadCount).Returns(1);
            mockAgentSettings.Setup(s => s.FlowThreadPatternName).Returns("constant");
            mockAgentSettings.Setup(s => s.AgentFlowFilepath).Returns("test_path");

            environment.AgentConfig
                       .MockObject.Setup(c => c.AgentSettings)
                       .Returns(environment.AgentSettings.Instance);
            environment.WithAgentConfig(environment.AgentConfig.Instance);

            var testFlow = TestFlow;
            environment.PluginManager
                       .MockObject
                       .Setup(p => p.GetClassType(testFlow.Steps[0].Function.AssemblyQualifiedClassName))
                       .Returns(typeof(FakeFunction));

            environment.PluginManager
                      .MockObject
                      .Setup(p => p.GetPlugin(testFlow.Steps[0].Function.AssemblyName))
                      .Returns(GetType().GetTypeInfo().Assembly);

            environment.FlowManager
                       .MockObject
                       .Setup(f => f.GetFlow("test_flow"))
                       .Returns(testFlow);

            environment.FlowProvider
                       .MockObject
                       .Setup(p => p.GetFlow("test_path"))
                       .Returns(testFlow);

            environment.StartApplication();
            var cts = new CancellationTokenSource();
            var appTask = environment.Application.RunApplicationAsync(cts.Token);
            await appTask;
            FakeFunction.CreatedInstances.Any().Should().BeTrue("No FakeFunction instances found");
            FakeFunction.CreatedInstances.Max().Value.CallList.Any().Should().BeTrue("No methods invoked on FakeFunction instance");
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
