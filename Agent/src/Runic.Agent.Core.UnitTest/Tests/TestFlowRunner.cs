using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestFlowRunner
    {
        private TestEnvironment _testEnvironment { get; set; }

        [TestInitialize]
        public void Init()
        {
            _testEnvironment = new TestEnvironment();
        }

        [TestMethod]
        public async Task ExcuteFlowRunner_ExecutesFunctionMultipleTimes()
        {
            var flow = new Flow()
            {
                Name = "",
                StepDelayMilliseconds = 10,
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "test",
                            AssemblyQualifiedClassName = "Runic.Agent.Core.UnitTest.TestUtility.FakeFunction",
                            FunctionName = "Login"
                        }
                    }
                }
            };

            var pluginManager = new Mock<IPluginManager>();
            var fakeFunction = new FakeFunction();
            pluginManager.Setup(p => p.GetInstance(flow.Steps[0].Function.AssemblyQualifiedClassName))
                         .Returns(fakeFunction);

            var functionFactory = 
                new FunctionFactory( 
                    pluginManager.Object, 
                    _testEnvironment.Stats.Object, 
                    _testEnvironment.DataService.Object, 
                    _testEnvironment.LoggerFactory);
            var harness = new Mock<CucumberHarness>();

            int maxErrors = 0;
            var flowRunner = new FlowRunner(
                functionFactory, 
                new CucumberHarness(_testEnvironment.PluginManager.Object), 
                flow, 
                _testEnvironment.LoggerFactory, 
                maxErrors);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(2000);
            try
            {
                await flowRunner.ExecuteFlowAsync(cts.Token);
            }
            catch (AggregateException)
            {
                //all g
            }
            
            // three method calls per iteration, at least 2 iterations = 6
            Assert.IsTrue(fakeFunction.CallList.Count > 6, $"{fakeFunction.CallList.Count} call count was less than 6");
        }

        [TestMethod]
        public async Task ExcuteFlowRunner_ExecutesStepsInOrder()
        {
            var flow = new Flow()
            {
                Name = "",
                StepDelayMilliseconds = 10,
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "1",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "test",
                            AssemblyQualifiedClassName = "Runic.Agent.Core.UnitTest.TestUtility.FakeFunction",
                            FunctionName = "Login"
                        }
                    },
                    new Step()
                    {
                        StepName = "2",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "test",
                            AssemblyQualifiedClassName = "Runic.Agent.Core.UnitTest.TestUtility.FakeFunction",
                            FunctionName = "Register"
                        }
                    }
                }
            };

            var pluginManager = new Mock<IPluginManager>();
            var fakeFunction = new FakeFunction();
            pluginManager.Setup(p => p.GetInstance(flow.Steps[0].Function.AssemblyQualifiedClassName))
                         .Returns(fakeFunction);

            var functionFactory =
                new FunctionFactory(
                    pluginManager.Object,
                    _testEnvironment.Stats.Object,
                    _testEnvironment.DataService.Object,
                    _testEnvironment.LoggerFactory);
            var harness = new Mock<CucumberHarness>();

            int maxErrors = 0;
            var flowRunner = new FlowRunner(
                functionFactory,
                new CucumberHarness(_testEnvironment.PluginManager.Object),
                flow,
                _testEnvironment.LoggerFactory,
                maxErrors);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(300);
            try
            {
                await flowRunner.ExecuteFlowAsync(cts.Token);
            }
            catch (AggregateException)
            {
                //all g
            }

            // three method calls per iteration, at least 2 iterations = 6
            Assert.IsTrue(fakeFunction.CallList.Count > 6, $"{fakeFunction.CallList.Count} call count was less than 6");
            Assert.IsTrue(fakeFunction.CallList[1].InvocationTarget == "Login", $"steps not executed in order");
            Assert.IsTrue(fakeFunction.CallList[4].InvocationTarget == "Register", $"steps not executed in order");
        }

        [TestMethod]
        public async Task ExcuteFlowRunner_ExecutesStringStepsInOrder()
        {
            var flow = new Flow()
            {
                Name = "",
                StepDelayMilliseconds = 10,
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "ReturnFoo",
                        GetNextStepFromFunctionResult = true,
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "test",
                            AssemblyQualifiedClassName = "Runic.Agent.Core.UnitTest.TestUtility.FakeFunction",
                            FunctionName = "ReturnFoo"
                        }
                    },
                    new Step()
                    {
                        StepName = "ReturnBar",
                        GetNextStepFromFunctionResult = true,
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "test",
                            AssemblyQualifiedClassName = "Runic.Agent.Core.UnitTest.TestUtility.FakeFunction",
                            FunctionName = "ReturnBar"
                        }
                    }
                }
            };

            var pluginManager = new Mock<IPluginManager>();
            var fakeFunction = new FakeFunction();
            pluginManager.Setup(p => p.GetInstance(flow.Steps[0].Function.AssemblyQualifiedClassName))
                         .Returns(fakeFunction);

            var functionFactory =
                new FunctionFactory(
                    pluginManager.Object,
                    _testEnvironment.Stats.Object,
                    _testEnvironment.DataService.Object,
                    _testEnvironment.LoggerFactory);
            var harness = new Mock<CucumberHarness>();

            int maxErrors = 0;
            var flowRunner = new FlowRunner(
                functionFactory,
                new CucumberHarness(_testEnvironment.PluginManager.Object),
                flow,
                _testEnvironment.LoggerFactory,
                maxErrors);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(300);
            try
            {
                await flowRunner.ExecuteFlowAsync(cts.Token);
            }
            catch (AggregateException)
            {
                //all g
            }

            // three method calls per iteration, at least 2 iterations = 6
            Assert.IsTrue(fakeFunction.CallList.Count > 6, $"{fakeFunction.CallList.Count} call count was less than 6");
            Assert.IsTrue(fakeFunction.CallList[1].InvocationTarget == "ReturnFoo", $"steps not executed in order");
            Assert.IsTrue(fakeFunction.CallList[4].InvocationTarget == "ReturnBar", $"steps not executed in order");
        }
    }
}
