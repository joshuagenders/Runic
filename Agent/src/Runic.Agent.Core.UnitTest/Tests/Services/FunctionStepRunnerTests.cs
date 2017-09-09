using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Models;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.Services
{
    [TestClass]
    public class FunctionStepRunnerTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenFunctionStepRunnerIsExecuted_MethodsAreInvoked()
        {
            var environment = new TestEnvironment();
            var functionFactory = new Mock<IFunctionFactory>();
            var mockDataService = new Mock<IDataService>();
            var functionHarness = new Core.FunctionHarness.FunctionHarness(new Mock<IEventService>().Object, mockDataService.Object);
            mockDataService.Setup(d => d.GetParams(It.IsAny<string[]>(), It.IsAny<MethodInfo>())).Returns(new object[] { });

            var fakeFunction = new FakeFunction();
            var step = new Step()
            {
                StepName = "step1",
                Function = new FunctionInformation()
                {
                    FunctionName = "Login"
                }
            };
            functionHarness.Bind(fakeFunction, step);

            functionFactory.Setup(f => f.CreateFunction(
                    It.IsAny<Step>(),
                    It.IsAny<Framework.Models.TestContext>()))
                .Returns(functionHarness);

            var startTime = DateTime.Now;
            environment.DatetimeService.Setup(d => d.Now).Returns(startTime);
            var functionRunner = new FunctionStepRunnerService(functionFactory.Object, environment.DatetimeService.Object);
            environment.DatetimeService.Setup(d => d.Now).Returns(startTime.AddMinutes(1));
            var result = await functionRunner.ExecuteStepAsync(step);
            result.Success.Should().BeTrue();
            result.NextStep.Should().BeNullOrEmpty();
            fakeFunction.CallList.Count.Should().Be(3);
        }
    }
}
