using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.StepController;
using Runic.Agent.ExampleTest.Functions;
using Runic.Agent.Standalone.Test.TestUtility;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
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
            var flow = EvenStepDistributionFlow;
            SetupTestEnvironment(flow);

            var startTime = DateTime.Now;
            _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1200);
            _environment.StartApplication();
            var appTask = _environment.Application.RunApplicationAsync(cts.Token);

            Thread.Sleep(100);

            _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime.AddSeconds(10));
            await appTask;
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
            var flow = TestFlow;
            SetupTestEnvironment(flow, "graph");

            var startTime = DateTime.Now;
            _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1200);
            _environment.StartApplication();
            var appTask = _environment.Application.RunApplicationAsync(cts.Token);

            Thread.Sleep(100);

            _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime.AddSeconds(10));
            await appTask;

            //todo assert thread levels

            AssertLogsCreatedWithoutErrors();
            AssertSuccessfulTestResult(flow);
        }

        [TestCategory("FunctionalTest")]
        [TestMethod]
        public async Task WhenStepRepeats_ThenStepIsRepeated()
        {
            var flow = StepRepeatFlow;
            SetupTestEnvironment(flow);

            var startTime = DateTime.Now;
            _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1200);
            _environment.StartApplication();
            var appTask = _environment.Application.RunApplicationAsync(cts.Token);

            Thread.Sleep(100);

            _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime.AddSeconds(10));
            await appTask;

            //todo assert step repeated

            AssertLogsCreatedWithoutErrors();
            AssertSuccessfulTestResult(flow);
        }

        [TestCategory("FunctionalTest")]
        [TestMethod]
        public async Task WhenStringReturn_ThenFlowFollowsReturnStringValues()
        {
            var flow = StringReturnFlow;
            SetupTestEnvironment(flow);

            var startTime = DateTime.Now;
            _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1200);
            _environment.StartApplication();
            var appTask = _environment.Application.RunApplicationAsync(cts.Token);

            Thread.Sleep(100);

            _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime.AddSeconds(10));
            await appTask;

            AssertLogsCreatedWithoutErrors();
            AssertSuccessfulTestResult(flow); ;
        }

        [TestCategory("FunctionalTest")]
        [TestMethod]
        public async Task WhenFunctionTestIsExecuted_MultipleStepsAreExecutedAndSuccessful()
        {
            var flow = TestFlow;
            SetupTestEnvironment(flow);

            var startTime = DateTime.Now;
            _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1200);
            _environment.StartApplication();
            var appTask = _environment.Application.RunApplicationAsync(cts.Token);

            Thread.Sleep(100);

            _environment.DateTimeService.MockObject.Setup(d => d.Now).Returns(startTime.AddSeconds(10));
            await appTask;
            AssertLogsCreatedWithoutErrors();
            AssertSuccessfulTestResult(flow);
        }

        private void AssertLogsCreatedWithoutErrors()
        {
            _environment.EventHandler.MockObject.Verify(l => l.Info(It.IsAny<string>(), null));
            _environment.EventHandler.MockObject.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        private void AssertSuccessfulTestResult(Flow flow)
        {
            _environment.EventHandler.MockObject.Verify(r => r.OnFlowStart(flow));
            _environment.EventHandler.MockObject.Verify(r => r.OnTestResult(It.Is<Result>(f => f.Success)));
            _environment.EventHandler.MockObject.Verify(r => r.OnThreadChange(flow, _environment.AgentConfig.Instance.AgentSettings.FlowThreadCount));
          
            _environment.EventHandler.MockObject.Verify(r => r.OnThreadChange(flow, 0));
            _environment.EventHandler.MockObject.Verify(r => r.OnFlowComplete(flow));
        }

        public void SetupTestEnvironment(Flow flow, string patternType = "constant")
        {
            var mockAgentSettings = _environment.AgentSettings.MockObject;
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
            _environment.AgentConfig
                        .MockObject
                        .Setup(c => c.AgentSettings)
                        .Returns(_environment.AgentSettings.Instance);
            _environment.PluginManager
                        .MockObject
                        .Setup(s => s.GetInstance("Runic.Agent.ExampleTest.Functions.ArticleFunctions"))
                        .Returns(new ArticleFunctions());

            _environment.FlowProvider
                        .MockObject
                        .Setup(p => p.GetFlow("test_path"))
                        .Returns(flow);
        }

        #region TestFlows
        private Flow StringReturnFlow => new Flow()
        {
            Name = "StringReturnFlow",
            StepDelayMilliseconds = 10,
            Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step 1",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "StringReturn",
                            GetNextStepFromFunctionResult = true,
                            Parameters = new List<string>()
                            {
                                "Step 4"
                            }
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 2",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "StringReturn",
                            GetNextStepFromFunctionResult = true,
                            Parameters = new List<string>()
                            {
                                "Step 1" 
                            }
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 3",
                        
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "StringReturn",
                            GetNextStepFromFunctionResult = true,
                            Parameters = new List<string>()
                            {
                                "Step 2"
                            }
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 4",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "StringReturn",
                             GetNextStepFromFunctionResult = true,
                            Parameters = new List<string>()
                            {
                                 "Step 3"
                            }
                        }
                    }
                }
        };

        private Flow StepDistributionFlow => new Flow()
        {
            Name = "StepRepeatFlow",
            StepDelayMilliseconds = 10,
            Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step 1",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 50,
                        },
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "Step1",
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 2",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 30,
                        },
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "Step2",
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 3",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 20,
                        },
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "Step3",
                        }
                    }
                }
        };

        private Flow EvenStepDistributionFlow => new Flow()
        {
            Name = "StepRepeatFlow",
            StepDelayMilliseconds = 10,
            Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step 1",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 50,
                        },
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "Step1"
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 2",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 50,
                        },
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "Step2"
                        }
                    }
                }
        };

        private Flow StepRepeatFlow => new Flow()
        {
            Name = "StepRepeatFlow",
            StepDelayMilliseconds = 10,
            Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step 1",
                        RepeatCount = 3,
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "OpenArticle",
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 2",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "OpenArticle",
                        }
                    }
                }
        };

        private Flow TestFlow => new Flow()
        {
            Name = "SystemTestFlow",
            StepDelayMilliseconds = 10,
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
                    },
                    new Step()
                    {
                        StepName = "Step 2",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "OpenArticle",
                            Parameters = new List<string>()
                            {
                                 "Greyfriars_Bobby" }
                            }
                        }
                    }
            };
    }
        #endregion
}
