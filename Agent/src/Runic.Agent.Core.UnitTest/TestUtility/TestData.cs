using Runic.Agent.Framework.Models;
using System.Collections.Generic;

namespace Runic.Agent.Core.UnitTest.TestUtility
{
    public static class TestData
    {
        public static Flow GetTestFlowSingleStep => new Flow()
        {
            Name = "Test",
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "step1",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = TestConstants.AssemblyName,
                            FunctionName = TestConstants.FunctionName,
                            AssemblyQualifiedClassName = TestConstants.AssemblyQualifiedClassName
                        }
                    }
                }
            };

        public static Flow GetTestFlowSingleStepRepeatTwice => new Flow()
        {
            Name = "Test",
            Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "step1",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = TestConstants.AssemblyName,
                            FunctionName = TestConstants.FunctionName,
                            AssemblyQualifiedClassName = TestConstants.AssemblyQualifiedClassName
                        },
                        RepeatCount = 2,
                    }
                }
        };

        public static Flow GetTestFlowSingleStepLooping => new Flow()
        {
            Name = "Test",
            Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "step1",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = TestConstants.AssemblyName,
                            FunctionName = TestConstants.FunctionName,
                            AssemblyQualifiedClassName = TestConstants.AssemblyQualifiedClassName
                        }
                    }
                }
        };

        public static Flow GetTestFlowTwoSteps => new Flow()
        {
            Name = "Test",
            Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "step1",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = TestConstants.AssemblyName,
                            FunctionName = TestConstants.FunctionName,
                            AssemblyQualifiedClassName = TestConstants.AssemblyQualifiedClassName
                        }
                    },
                    new Step()
                    {
                        StepName = "step2",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = TestConstants.AssemblyName,
                            FunctionName = TestConstants.FunctionName,
                            AssemblyQualifiedClassName = TestConstants.AssemblyQualifiedClassName
                        }
                    }
                }
        };
    }
}
