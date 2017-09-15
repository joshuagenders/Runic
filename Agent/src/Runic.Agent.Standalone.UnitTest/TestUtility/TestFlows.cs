using Runic.Agent.Framework.Models;
using System.Collections.Generic;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public static class TestFlows
    {
        public static Flow StringReturnFlow => new Flow()
        {
            Name = "StringReturnFlow",
            StepDelayMilliseconds = 10,
            Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step 1",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "StringReturn",
                            GetNextStepFromFunctionResult = true,
                            InputParameters = new List<string>()
                            {
                                "Step 4"
                            }
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 2",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "StringReturn",
                            GetNextStepFromFunctionResult = true,
                            InputParameters = new List<string>()
                            {
                                "Step 1"
                            }
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 3",

                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "StringReturn",
                            GetNextStepFromFunctionResult = true,
                            InputParameters = new List<string>()
                            {
                                "Step 2"
                            }
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 4",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "StringReturn",
                             GetNextStepFromFunctionResult = true,
                            InputParameters = new List<string>()
                            {
                                 "Step 3"
                            }
                        }
                    }
                }
        };

        public static Flow EvenStepDistributionFlow => new Flow()
        {
            Name = "StepRepeatFlow",
            StepDelayMilliseconds = 10,
            Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step 1",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 50,
                        },
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "Step1"
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 2",
                        Distribution = new Distribution()
                        {
                            DistributionAllocation = 50,
                        },
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "Step2"
                        }
                    }
                }
        };

        public static Flow StepRepeatFlow => new Flow()
        {
            Name = "StepRepeatFlow",
            StepDelayMilliseconds = 10,
            Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step 1",
                        RepeatCount = 3,
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "OpenArticle",
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 2",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "OpenArticle",
                        }
                    }
                }
        };

        public static Flow SystemTestFlow => new Flow()
        {
            Name = "SystemTestFlow",
            StepDelayMilliseconds = 10,
            Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step 1",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "OpenArticle"
                        }
                    },
                    new Step()
                    {
                        StepName = "Step 2",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.Agent.Examples",
                            AssemblyQualifiedClassName = "Runic.Agent.ExampleTest.Functions.ArticleFunctions",
                            FunctionName = "OpenArticle",
                            InputParameters = new List<string>()
                            {
                                 "Greyfriars_Bobby"
                            }
                        }
                    }
            }
        };
    }
}
