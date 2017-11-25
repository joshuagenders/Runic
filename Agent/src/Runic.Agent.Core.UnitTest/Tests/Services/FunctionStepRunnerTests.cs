using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Harness;
using Runic.Agent.UnitTest.TestUtility;
using Runic.Agent.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Core.Services;

namespace Runic.Agent.Core.UnitTest.Tests.Services
{
    [TestClass]
    public class FunctionStepRunnerTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenFunctionStepRunnerIsExecuted_MethodsAreInvoked()
        {
            var fakeFunction = new FakeFunction();
            var step = new Step()
            {
                StepName = "Step 1",
                Function = new MethodInformation()
                {
                    MethodName = "Login",
                    AssemblyQualifiedClassName = ""
                }
            };

            var functionHarness = new Harness.FunctionHarness(fakeFunction, step);

            var functionFactory = new Mock<IFunctionFactory>();
            functionFactory.Setup(f => f.CreateFunction(It.IsAny<Step>(), It.IsAny<Models.TestContext>()))
                           .Returns(functionHarness);

            var startTime = DateTime.Now;
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            var datetimeService = new Mock<IDatetimeService>();
            datetimeService.Setup(d => d.Now).Returns(startTime);

            var functionRunner = new FunctionStepRunnerService(functionFactory.Object, datetimeService.Object);
            
            var result = await functionRunner.ExecuteStepAsync(step, cts.Token);

            datetimeService.Setup(d => d.Now).Returns(startTime.AddMinutes(1));

            result.Success.Should().BeTrue();
            result.NextStep.Should().BeNullOrEmpty();
        }
    }
}
