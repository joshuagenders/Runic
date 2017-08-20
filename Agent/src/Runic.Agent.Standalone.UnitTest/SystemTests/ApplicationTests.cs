using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services;
using Runic.Agent.ExampleTest.Functions;
using Runic.Agent.Standalone.Configuration;
using Runic.Agent.Standalone.Providers;
using Runic.Agent.Standalone.Test.TestUtility;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone.Test.SystemTests
{
    [TestClass]
    public class ApplicationTests
    {
        private TestEnvironmentBuilder _environment { get; set; }

        [TestInitialize]
        public void Init()
        {
            _environment = 
                new SystemTestEnvironmentBuilder()
                    .New();
        }

        [TestMethod]
        public async Task WhenFunctionTestIsExecuted_MultipleStepsAreExecutedAndSuccessful()
        {
            var flow = TestFlow;
            SetupTestEnvironment(flow);

            _environment.StartApplication();
            await _environment.Application.RunApplicationAsync();

            _environment.GetMock<ITestResultHandler>()
                        .Verify(t => t.OnFlowStart(flow));
            _environment.GetMock<ITestResultHandler>()
                        .Verify(t => t.OnFunctionComplete(It.Is<FunctionResult>(f => f.Success == true)));
        }

        public void SetupTestEnvironment(Flow flow, string patternType = "constant")
        {
            _environment.With(_environment.GetMock<IPluginManager>().Object);
            _environment.With(_environment.GetMock<IFlowProvider>().Object);
            _environment.With<IDatetimeService>(new DateTimeService());

            var mockAgentSettings = _environment.GetMock<IAgentSettings>();
            mockAgentSettings.Setup(s => s.FlowPatternExecutionId).Returns("test_execution_id");
            mockAgentSettings.Setup(s => s.FlowThreadPatternName).Returns(patternType);
            mockAgentSettings.Setup(s => s.AgentFlowFilepath).Returns("test_path");
            mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(5);

            switch (patternType.ToLowerInvariant())
            {
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
            _environment.GetMock<IPluginManager>()
                        .Setup(s => s.GetInstance("Runic.Agent.ExampleTest.Functions.ArticleFunctions"))
                        .Returns(new ArticleFunctions());

            _environment.GetMock<IFlowProvider>()
                        .Setup(p => p.GetFlow("test_path"))
                        .Returns(flow);
            ArticleFunctions.RuneClient = _environment.Get<IRuneClient>();
            ArticleFunctions.StatsClient = _environment.Get<IStatsClient>();
        }

        private Flow TestFlow => new Flow()
        {
            Name = "SystemTestFlow",
            StepDelayMilliseconds = 300,
            Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step 1",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "OpenArticle"
                        }
                    }
                }
        };
    }
}
