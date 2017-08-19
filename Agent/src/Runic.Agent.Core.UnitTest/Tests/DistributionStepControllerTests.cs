using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class DistributionStepControllerTests
    {

        [TestMethod]
        public void WhenSingleStepIsUsed_NextStepAlwaysReturnsStep()
        {
            var flow = new Flow()
            {
                Name = "TestFlow",
                Steps = new List<Step>()
                    {
                        new Step()
                        {
                            StepName = "Step1",
                            Distribution = new Distribution()
                            {
                                DistributionAllocation = 100
                            }
                        }
                    }
            };
            var controller = new DistributionStepController(flow.Steps);
            Step step = controller.GetNextStep(null);
            step.Should().Be(flow.Steps[0]);
            for (var i = 0; i < 3; i++)
            {
                var step2 = controller.GetNextStep(new FunctionResult()
                {
                    Step = flow.Steps[0],
                    StepName = "Step1",
                    Success = true
                });
                step2.Should().Be(flow.Steps[0]);
            }
        }

        [TestMethod]
        public void WhenStepsHaveNoCommonFactor_ThenQueueSizeIsSumOfDistributionAllocation()
        {
            var flow = new Flow()
            {
                Name = "TestFlow",
                Steps = new List<Step>()
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
                    }
            };
            var controller = new DistributionStepController(flow.Steps);
            
            Step step = controller.GetNextStep(null);
            controller.QueueSize.Should().Be(200);
        }

        [TestMethod]
        public void WhenQueueIsEmptied_QueueIsRefreshed()
        {
            var flow = new Flow()
            {
                Name = "TestFlow",
                Steps = new List<Step>()
                    {
                        new Step()
                        {
                            StepName = "Step1",
                            Distribution = new Distribution()
                            {
                                DistributionAllocation = 1
                            }
                        }
                    }
            };
            var controller = new DistributionStepController(flow.Steps);

            Step step = controller.GetNextStep(null);
            step.Should().Be(flow.Steps[0]);
            controller.QueueSize.Should().Be(0);
            step = controller.GetNextStep(new FunctionResult()
            {
                Step = flow.Steps[0],
                StepName = "Step1",
                Success = true
            });
            step.Should().Be(flow.Steps[0]);
        }

        [TestMethod]
        public void WhenStepsHaveCommonFactor_ThenQueueSizeIsSumOfDitributionAllocationOverGCF()
        {
            var flow = new Flow()
            {
                Name = "TestFlow",
                Steps = new List<Step>()
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
                    }
            };
            var controller = new DistributionStepController(flow.Steps);

            Step step = controller.GetNextStep(null);
            step.Should().Be(flow.Steps[0]);
            controller.QueueSize.Should().Be(1);
            step = controller.GetNextStep(new FunctionResult()
            {
                Step = flow.Steps[0],
                StepName = "Step1",
                Success = true
            });
            step.Should().Be(flow.Steps[1]);
        }
    }
}
