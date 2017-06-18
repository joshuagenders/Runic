﻿using Runic.Framework.Models;
using System.Collections.Generic;

namespace Runic.Agent.UnitTest.TestUtility
{
    public class TestData
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
                        Repeat = 2,
                        EvaluateSuccessOnRepeat = false
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
                        },
                        NextStepOnFailure = "step1",
                        NextStepOnSuccess = "step1",
                        EvaluateSuccessOnRepeat = false,
                        
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