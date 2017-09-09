using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.Services
{
    [TestClass]
    public class RunnerServiceTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenExecutingFlow_ThenMethodsInvokedAndLogged()
        {
            var mockPluginManager = new Mock<IPluginManager>();
            var mockFunctionFactory = new Mock<IFunctionFactory>();
            var mockDatetimeService = new Mock<IDatetimeService>();
            var mockEventService = new Mock<IEventService>();
            var mockDataService = new Mock<IDataService>();

            var runnerService = new RunnerService(mockPluginManager.Object, mockFunctionFactory.Object, mockDatetimeService.Object, mockEventService.Object);
            var flow = new Flow()
            {
                Name = "flow",
                StepDelayMilliseconds = 10,
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step1",
                        Function = new FunctionInformation()
                        {
                            FunctionName = "Login",
                            AssemblyQualifiedClassName = "ClassName",
                            AssemblyName = "AssemblyName",
                            Parameters = new List<string>()
                        }
                    }
                }
            };
            var cts = new CancellationTokenSource();
            var fakeFunction = new FakeFunction();
            var functionHarness = new Core.FunctionHarness.FunctionHarness(mockEventService.Object, mockDataService.Object);
            functionHarness.Bind(fakeFunction, flow.Steps[0]);
            mockPluginManager.Setup(p => p.GetInstance("ClassName")).Returns(fakeFunction);
            mockFunctionFactory.Setup(f => f.CreateFunction(It.IsAny<Step>(), It.IsAny<Framework.Models.TestContext>()))
                               .Returns(functionHarness);
            mockDataService.Setup(d => d.GetParams(It.IsAny<string[]>(), It.IsAny<MethodInfo>())).Returns(new object[] { });

            cts.CancelAfter(500);
            await runnerService.ExecuteFlowAsync(flow, cts.Token);
            
            fakeFunction.CallList.Should().Contain(f => f.InvocationTarget == "BeforeEach", "BeforeEach");
            fakeFunction.CallList.Should().Contain(f => f.InvocationTarget == "Login", "Login");
            fakeFunction.CallList.Should().Contain(f => f.InvocationTarget == "AfterEach", "AfterEach");
        }
    }
}
