using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Agent.TestUtility;
using Runic.Framework.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.Services
{
    [TestClass]
    public class FunctionStepRunnerTests
    {
        private TestEnvironmentBuilder _environment { get; set; }

        [TestInitialize]
        public void Init()
        {
            _environment = new UnitEnvironment();
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenFunctionStepRunnerIsExecuted_MethodsAreInvoked()
        {
            var functionHarness = new Core.FunctionHarness.FunctionHarness(_environment.Get<IEventService>(), _environment.Get<IDataService>());

            var fakeFunction = new FakeFunction();
            var step = TestData.GetTestFlowSingleStep.Steps[0];
            functionHarness.Bind(fakeFunction, step);

            _environment.GetMock<IFunctionFactory>()
                        .Setup(f => f.CreateFunction(It.IsAny<Step>(), It.IsAny<Framework.Models.TestContext>()))
                        .Returns(functionHarness);

            var startTime = DateTime.Now;
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            _environment.DatetimeService.MockObject.Setup(d => d.Now).Returns(startTime);

            var functionRunner = new FunctionStepRunnerService(_environment.Get<IFunctionFactory>(), _environment.Get<IDatetimeService>());
            
            var result = await functionRunner.ExecuteStepAsync(step, cts.Token);

            _environment.DatetimeService.MockObject.Setup(d => d.Now).Returns(startTime.AddMinutes(1));

            result.Success.Should().BeTrue();
            result.NextStep.Should().BeNullOrEmpty();
        }
    }
}
