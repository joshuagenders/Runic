//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using Runic.Agent.Core.PluginManagement;
//using Runic.Agent.Core.FunctionHarness;
//using Runic.Agent.Core.Services;
//using Runic.Agent.ExampleTest.Functions;
//using Runic.Agent.Standalone.Configuration;
//using Runic.Agent.Standalone.Providers;
//using Runic.Agent.Standalone.Test.TestUtility;
//using Runic.Framework.Clients;
//using Runic.Framework.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System;

//namespace Runic.Agent.Standalone.Test.FunctionalTests
//{
//    [TestClass]
//    public class ApplicationTests
//    {
//        private TestEnvironmentBuilder _environment { get; set; }

//        [TestInitialize]
//        public void Init()
//        {
//            _environment = 
//                new SystemTestEnvironmentBuilder()
//                    .New();
//        }
        
//        [TestCategory("FunctionalTest")]
//        [TestMethod]
//        public async Task WhenStepDistributionIsEven_ThenStepExecutionIsEven()
//        {
//            var flow = EvenStepDistributionFlow;
//            SetupTestEnvironment(flow);

//            _environment.StartApplication();
//            var startTime = new DateTime();
//            _environment.DatetimeService.MockObject.Setup(d => d.Now).Returns(startTime);
//            var applicationTask = _environment.Application.RunApplicationAsync();
//            _environment.DatetimeService.MockObject.Setup(d => d.Now).Returns(startTime.AddSeconds(1));
//            await applicationTask;
//            //Todo assert distribution even

//            AssertLogsCreatedWithoutErrors();
//            AssertStatsPushed(flow);
//            AssertSuccessfulTestResult(flow);

//            //TODO assertions on:
//            //step distribution

//        }

//        [TestCategory("FunctionalTest")]
//        [TestMethod]
//        public async Task WhenGraphPatternIsExecuted_ThenThreadLevelsAreSet()
//        {
//            var flow = TestFlow;
//            SetupTestEnvironment(flow, "graph");

//            _environment.StartApplication();
//            await _environment.Application.RunApplicationAsync();

//            //todo assert thread levels

//            AssertLogsCreatedWithoutErrors();
//            AssertStatsPushed(flow);
//            AssertSuccessfulTestResult(flow);
//        }

//        [TestCategory("FunctionalTest")]
//        [TestMethod]
//        public async Task WhenStepRepeats_ThenStepIsRepeated()
//        {
//            var flow = StepRepeatFlow;
//            SetupTestEnvironment(flow);

//            _environment.StartApplication();
//            await _environment.Application.RunApplicationAsync();

//            //todo assert step repeated

//            AssertLogsCreatedWithoutErrors();
//            AssertStatsPushed(flow);
//            AssertSuccessfulTestResult(flow);
//        }

//        [TestCategory("FunctionalTest")]
//        [TestMethod]
//        public async Task WhenStringReturn_ThenFlowFollowsReturnStringValues()
//        {
//            var flow = StringReturnFlow;
//            SetupTestEnvironment(flow);

//            _environment.StartApplication();
//            var task = _environment.Application.RunApplicationAsync();
//            await task;

//            AssertLogsCreatedWithoutErrors();
//            AssertStatsPushed(flow);
//            AssertSuccessfulTestResult(flow);
//        }

//        [Ignore]
//        [TestCategory("FunctionalTest")]
//        [TestMethod]
//        public void WhenFailingScenarioReachesFailureLimit_ThenTestCompletesWithErrors()
//        {
//            throw new NotImplementedException();
//        }

//        [TestCategory("FunctionalTest")]
//        [TestMethod]
//        public async Task WhenFunctionTestIsExecuted_MultipleStepsAreExecutedAndSuccessful()
//        {
//            var flow = TestFlow;
//            SetupTestEnvironment(flow);
//            _environment.StartApplication();
//            await _environment.Application.RunApplicationAsync();

//            AssertLogsCreatedWithoutErrors();
//            AssertStatsPushed(flow);
//            AssertSuccessfulTestResult(flow);
//        }

//        private  void AssertStatsPushed(Flow flow)
//        {
//            var functionName = flow.Steps[0].Function.FunctionName;
//            _environment.StatsClient.MockObject.Verify(s => s.CountFlowAdded(flow.Name));
//            _environment.StatsClient.MockObject.Verify(s => s.CountFunctionSuccess(functionName));
//            //todo fix bugs in completion
//            //_environment.StatsClient.MockObject.Verify(s => s.SetThreadLevel(flow.Name, _environment.AgentConfig.Instance.AgentSettings.FlowThreadCount));
//        }

//        private void AssertLogsCreatedWithoutErrors()
//        {
//            _environment.LoggingHandler.MockObject.Verify(l => l.Debug(It.IsAny<string>()));
//            _environment.LoggingHandler.MockObject.Verify(l => l.Error(It.IsAny<string>()), Times.Never);
//        }

//        private void AssertSuccessfulTestResult(Flow flow)
//        {
//            _environment.TestResultHandler.MockObject.Verify(r => r.OnFlowStart(flow));
//            _environment.TestResultHandler.MockObject.Verify(r => r.OnFunctionComplete(It.Is<FunctionResult>(f => f.Success)));
//            _environment.TestResultHandler.MockObject.Verify(r => r.OnThreadChange(flow, _environment.AgentConfig.Instance.AgentSettings.FlowThreadCount));
            
//            //todo fix bugs in completion
//            //_environment.TestResultHandler.MockObject.Verify(r => r.OnThreadChange(flow, 0));
//            //_environment.TestResultHandler.MockObject.Verify(r => r.OnFlowComplete(flow));

//        }

//        public void SetupTestEnvironment(Flow flow, string patternType = "constant")
//        {
//            _environment.With(_environment.GetMock<IPluginManager>().Object);
//            _environment.With(_environment.GetMock<IFlowProvider>().Object);
//            _environment.With<IDatetimeService>(new DateTimeService());

//            var mockAgentSettings = _environment.GetMock<IAgentSettings>();
//            mockAgentSettings.Setup(s => s.FlowPatternExecutionId).Returns("test_execution_id");
//            mockAgentSettings.Setup(s => s.FlowThreadPatternName).Returns(patternType);
//            mockAgentSettings.Setup(s => s.AgentFlowFilepath).Returns("test_path");
//            mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(5);

//            switch (patternType.ToLowerInvariant())
//            {
//                case "constant":
//                    mockAgentSettings.Setup(s => s.FlowThreadCount).Returns(1);
//                    mockAgentSettings.Setup(s => s.FlowDurationSeconds).Returns(3);
//                    break;
//                case "graph":
//                    mockAgentSettings.Setup(s => s.FlowPoints).Returns(new string[] { "0.1", "3.0" });
//                    break;
//                case "gradual":
//                    mockAgentSettings.Setup(s => s.FlowRampUpSeconds).Returns(0);
//                    mockAgentSettings.Setup(s => s.FlowRampDownSeconds).Returns(0);
//                    mockAgentSettings.Setup(s => s.FlowThreadCount).Returns(1);
//                    mockAgentSettings.Setup(s => s.FlowStepIntervalSeconds).Returns(1);
//                    break;
//            }
//            _environment.GetMock<IAgentConfig>()
//                        .Setup(c => c.AgentSettings)
//                        .Returns(_environment.Get<IAgentSettings>());
//            _environment.GetMock<IPluginManager>()
//                        .Setup(s => s.GetInstance("Runic.Agent.ExampleTest.Functions.ArticleFunctions"))
//                        .Returns(new ArticleFunctions());

//            _environment.GetMock<IFlowProvider>()
//                        .Setup(p => p.GetFlow("test_path"))
//                        .Returns(flow);
//            ArticleFunctions.RuneClient = _environment.Get<IRuneClient>();
//            ArticleFunctions.StatsClient = _environment.Get<IStatsClient>();
//        }

//        #region TestFlows
//        private Flow StringReturnFlow => new Flow()
//        {
//            Name = "StringReturnFlow",
//            StepDelayMilliseconds = 300,
//            Steps = new List<Step>()
//                {
//                    new Step()
//                    {
//                        StepName = "Step 1",
//                        GetNextStepFromFunctionResult = true,
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "StringReturn",
//                            //todo change dictionary to List<Tuple<Type, string>>
//                            Parameters = new Dictionary<string, Type>()
//                            {
//                                { "Step 4", typeof(string) }
//                            }
//                        }
//                    },
//                    new Step()
//                    {
//                        StepName = "Step 2",
//                        GetNextStepFromFunctionResult = true,
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "StringReturn",
//                            Parameters = new Dictionary<string, Type>()
//                            {
//                                { "Step 1", typeof(string) }
//                            }
//                        }
//                    },
//                    new Step()
//                    {
//                        StepName = "Step 3",
//                        GetNextStepFromFunctionResult = true,
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "StringReturn",
//                            Parameters = new Dictionary<string, Type>()
//                            {
//                                { "Step 2", typeof(string) }
//                            }
//                        }
//                    },
//                    new Step()
//                    {
//                        StepName = "Step 4",
//                        GetNextStepFromFunctionResult = true,
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "StringReturn",
//                            Parameters = new Dictionary<string, Type>()
//                            {
//                                { "Step 3", typeof(string) }
//                            }
//                        }
//                    }
//                }
//        };

//        private Flow StepDistributionFlow => new Flow()
//        {
//            Name = "StepRepeatFlow",
//            StepDelayMilliseconds = 300,
//            Steps = new List<Step>()
//                {
//                    new Step()
//                    {
//                        StepName = "Step 1",
//                        Distribution = new Distribution()
//                        {
//                            DistributionAllocation = 50,
//                        },
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "Step1",
//                        }
//                    },
//                    new Step()
//                    {
//                        StepName = "Step 2",
//                        Distribution = new Distribution()
//                        {
//                            DistributionAllocation = 30,
//                        },
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "Step2",
//                        }
//                    },
//                    new Step()
//                    {
//                        StepName = "Step 3",
//                        Distribution = new Distribution()
//                        {
//                            DistributionAllocation = 20,
//                        },
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "Step3",
//                        }
//                    }
//                }
//        };

//        private Flow EvenStepDistributionFlow => new Flow()
//        {
//            Name = "StepRepeatFlow",
//            StepDelayMilliseconds = 300,
//            Steps = new List<Step>()
//                {
//                    new Step()
//                    {
//                        StepName = "Step 1",
//                        Distribution = new Distribution()
//                        {
//                            DistributionAllocation = 50,
//                        },
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "Step1"
//                        }
//                    },
//                    new Step()
//                    {
//                        StepName = "Step 2",
//                        Distribution = new Distribution()
//                        {
//                            DistributionAllocation = 50,
//                        },
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "Step2"
//                        }
//                    }
//                }
//        };

//        private Flow StepRepeatFlow => new Flow()
//        {
//            Name = "StepRepeatFlow",
//            StepDelayMilliseconds = 300,
//            Steps = new List<Step>()
//                {
//                    new Step()
//                    {
//                        StepName = "Step 1",
//                        Repeat = 3,
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "OpenArticle",
//                        }
//                    },
//                    new Step()
//                    {
//                        StepName = "Step 2",
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "OpenArticle",
//                        }
//                    }
//                }
//        };

//        private Flow TestFlow => new Flow()
//        {
//            Name = "SystemTestFlow",
//            StepDelayMilliseconds = 300,
//            Steps = new List<Step>()
//                {
//                    new Step()
//                    {
//                        StepName = "Step 1",
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "OpenArticle"
//                        }
//                    },
//                    new Step()
//                    {
//                        StepName = "Step 2",
//                        Function = new FunctionInformation()
//                        {
//                            AssemblyName = "Runic.Agent.Examples",
//                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
//                            FunctionName = "OpenArticle",
//                            Parameters = new Dictionary<string, Type>()
//                            {
//                                { "Greyfriars_Bobby", typeof(string) }
//                            }
//                        }
//                    }
//                }
//        };
//        #endregion
//    }
//}
