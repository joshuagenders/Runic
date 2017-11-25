using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.StepController;
using Runic.Agent.Core.Models;
using System.Collections.Generic;
using System.Linq;
using Runic.Agent.Harness;

namespace Runic.Agent.Core.UnitTest.Tests.StepController
{
    [TestClass]
    public class StandardStepControllerTests
    {
        private Journey _journey => new Journey()
        {
            Steps = new List<Step>()
                {
                    new Step() { StepName = "Step1", Function = new MethodInformation() },
                    new Step() { StepName = "Step2", Function = new MethodInformation() },
                }
        };

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenNullResult_ReturnsFirstStep()
        {
            var journey = _journey;
            var stepController = new StandardStepController(journey);
            var step = stepController.GetNextStep(null);
            step.Should().Be(journey.Steps.First());
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenStringReturnedAsNextStepFromFunction_ReturnsNextStep()
        {
            var journey = _journey;
            journey.Steps[0].Function.GetNextStepFromMethodResult = true;
            journey.Steps.Add(new Step() { StepName = "Step3" });
            var result = new FunctionResult()
            {
                Success = true,
                Step = journey.Steps[0],
                NextStep = "Step3"
            };
            var stepController = new StandardStepController(journey);
            stepController.GetNextStep(null);
            var step = stepController.GetNextStep(result);
            step.Should().Be(journey.Steps[2]);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingStepsLoopsAround_ReturnsSteps()
        {
            var journey = _journey;
            var stepController = new StandardStepController(journey);
            var step1 = stepController.GetNextStep(null);

            var result1 = new FunctionResult()
            {
                Success = true,
                StepName = step1.StepName,
                Step = _journey.Steps[0]
            };
            var step2 = stepController.GetNextStep(result1);

            var result2 = new FunctionResult()
            {
                Success = true,
                StepName = step2.StepName,
                Step = _journey.Steps[1]
            };
            var step3 = stepController.GetNextStep(result2);

            step1.Should().Be(journey.Steps[0]);
            step2.Should().Be(journey.Steps[1]);
            step3.Should().Be(journey.Steps[0]);
        }
    }
}
