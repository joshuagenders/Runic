﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.TestHarness.StepController;
using Runic.Agent.Framework.Models;
using System.Collections.Generic;
using System.Linq;
using Runic.Agent.TestHarness.Harness;

namespace Runic.Agent.Core.UnitTest.Tests.StepController
{
    [TestClass]
    public class StandardStepControllerTests
    {
        private Journey _flow => new Journey()
        {
            Steps = new List<Step>()
                {
                    new Step() { StepName = "Step1", Function = new FunctionInformation() },
                    new Step() { StepName = "Step2", Function = new FunctionInformation() },
                }
        };

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenNullResult_ReturnsFirstStep()
        {
            var flow = _flow;
            var stepController = new StandardStepController(flow);
            var step = stepController.GetNextStep(null);
            step.Should().Be(flow.Steps.First());
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenStringReturnedAsNextStepFromFunction_ReturnsNextStep()
        {
            var flow = _flow;
            flow.Steps[0].Function.GetNextStepFromFunctionResult = true;
            flow.Steps.Add(new Step() { StepName = "Step3" });
            var result = new FunctionResult()
            {
                Success = true,
                NextStep = "Step3"
            };
            var stepController = new StandardStepController(flow);
            stepController.GetNextStep(null);
            var step = stepController.GetNextStep(result);
            step.Should().Be(flow.Steps[2]);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingStepsLoopsAround_ReturnsSteps()
        {
            var flow = _flow;
            var stepController = new StandardStepController(flow);
            var step1 = stepController.GetNextStep(null);

            var result1 = new FunctionResult()
            {
                Success = true,
                StepName = step1.StepName
            };
            var step2 = stepController.GetNextStep(result1);

            var result2 = new FunctionResult()
            {
                Success = true,
                StepName = step2.StepName
            };
            var step3 = stepController.GetNextStep(result2);

            step1.Should().Be(flow.Steps[0]);
            step2.Should().Be(flow.Steps[1]);
            step3.Should().Be(flow.Steps[0]);
        }
    }
}