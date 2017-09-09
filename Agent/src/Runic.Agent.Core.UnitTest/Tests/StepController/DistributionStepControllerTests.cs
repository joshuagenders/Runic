using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.StepController;
using Runic.Framework.Models;
using System.Collections.Generic;

namespace Runic.Agent.Core.UnitTest.Tests.StepController
{
    [TestClass]
    public class DistributionStepControllerTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenSingleStepIsUsed_NextStepAlwaysReturnsStep()
        {
            var steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step1",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 100
                        }
                    }
                };
            var controller = new DistributionStepController(steps);
            Step step = controller.GetNextStep(null);
            step.Should().Be(steps[0]);
            for (var i = 0; i < 3; i++)
            {
                var step2 = controller.GetNextStep(new FunctionResult()
                {
                    Step = steps[0],
                    StepName = "Step1",
                    Success = true
                });
                step2.Should().Be(steps[0]);
            }
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenStepsHaveNoCommonFactor_ThenQueueSizeIsSumOfDistributionAllocation()
        {
            var steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step1",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 100
                        }
                    },
                    new Step()
                    {
                        StepName = "Step2",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 101
                        }
                    }
                    
                };
            var controller = new DistributionStepController(steps);
            
            controller.GetNextStep(null);
            controller.QueueSize.Should().Be(200);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenQueueIsEmptied_QueueIsRefreshed()
        {
            var steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step1",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 1
                        }
                    }
                };
            var controller = new DistributionStepController(steps);

            Step step = controller.GetNextStep(null);
            step.Should().Be(steps[0]);
            controller.QueueSize.Should().Be(0);
            step = controller.GetNextStep(new FunctionResult()
            {
                Step = steps[0],
                StepName = "Step1",
                Success = true
            });
            step.Should().Be(steps[0]);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenStepsHaveCommonFactor_ThenQueueSizeIsSumOfDitributionAllocationOverGCF()
        {
            var steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step1",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 12
                        }
                    },
                    new Step()
                    {
                        StepName = "Step2",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 12
                        }
                    }    
                };
            var controller = new DistributionStepController(steps);

            Step step = controller.GetNextStep(null);
            step.Should().Be(steps[0]);
            controller.QueueSize.Should().Be(1);
            step = controller.GetNextStep(new FunctionResult()
            {
                Step = steps[0],
                StepName = "Step1",
                Success = true
            });
            step.Should().Be(steps[1]);
        }
    }
}
