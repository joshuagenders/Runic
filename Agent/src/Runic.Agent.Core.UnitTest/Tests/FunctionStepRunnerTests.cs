using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Models;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class FunctionStepRunnerTests
    {
        [TestMethod]
        public async Task WhenFunctionStepRunnerISExecuted_MethodsAreInvoked()
        {
            var environment = new TestEnvironment();
            var functionFactory = new Mock<IFunctionFactory>();
            var functionHarness = new FunctionHarness.FunctionHarness(
                environment.Stats.Object,
                new Mock<ILoggingHandler>().Object);

            var fakeFunction = new FakeFunction();
            functionHarness.Bind(fakeFunction, "step1", "Login", false);
            functionFactory.Setup(f => f.CreateFunction(
                    It.IsAny<Step>(),
                    It.IsAny<Framework.Models.TestContext>()))
                .Returns(functionHarness);

            var functionRunner = new FunctionStepRunnerService(
                functionFactory.Object, 
                environment.DatetimeService.Object);

            var step = new Step()
            {
                StepName = "TestStep"
            };

            var result = await functionRunner.ExecuteStepAsync(step);
            result.Success.Should().BeTrue();
            result.NextStep.Should().BeNullOrEmpty();
            fakeFunction.CallList.Count.Should().Be(3);
        }
    }
}
