using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.TestHarness.UnitTest.TestUtility;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Harness;
using Runic.Agent.TestHarness.Services;
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
            var mockFunctionFactory = new Mock<IFunctionFactory>();
            var mockDatetimeService = new Mock<IDatetimeService>();
            
            var runnerService = new Person(mockFunctionFactory.Object, mockDatetimeService.Object, GetType().GetTypeInfo().Assembly);
            var flow = new Journey()
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
                            PositionalMethodParameterValues = new List<string>()
                        }
                    }
                }
            };
            var cts = new CancellationTokenSource();
            var fakeFunction = new FakeFunction();
            var functionHarness = new TestHarness.Harness.FunctionHarness();
            functionHarness.Bind(fakeFunction, flow.Steps[0]);
            mockFunctionFactory.Setup(f => f.CreateFunction(It.IsAny<Step>(), It.IsAny<Framework.Models.TestContext>()))
                               .Returns(functionHarness);
            
            cts.CancelAfter(500);
            await runnerService.PerformJourneyAsync(flow, cts.Token);
            
            fakeFunction.CallList.Should().Contain(f => f.InvocationTarget == "BeforeEach", "BeforeEach");
            fakeFunction.CallList.Should().Contain(f => f.InvocationTarget == "Login", "Login");
            fakeFunction.CallList.Should().Contain(f => f.InvocationTarget == "AfterEach", "AfterEach");
        }
    }
}
