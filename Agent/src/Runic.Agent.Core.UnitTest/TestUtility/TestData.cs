using Runic.Agent.Framework.Models;
using System;
using System.Collections.Generic;

namespace Runic.Agent.Core.UnitTest.TestUtility
{
    public static class TestData
    {
        public static Journey NewJourney => new Journey();
        public static Journey WithRepeatingStep(this Journey journey, int count)
        {
            if (journey.Steps == null)
                journey.Steps = new List<Step>();

            journey.Steps.Add(new Step()
            {
                StepName = $"Step {journey.Steps.Count + 1}",
                RepeatCount = count
            }.WithFunction());
            return journey;
        }

        public static Journey WithStep(this Journey journey)
        {
            if (journey.Steps == null)
                journey.Steps = new List<Step>();

            journey.Steps.Add(new Step()
            {
                StepName = $"Step {journey.Steps.Count + 1}"
            }.WithFunction());
            return journey;
        }

        public static Step WithFunction(this Step step)
        {
            step.Function = new FunctionInformation()
            {
                AssemblyName = "AssemblyName",
                AssemblyQualifiedClassName = "AssemblyQualifiedClassName",
                FunctionName = "FunctionName",
                GetNextStepFromFunctionResult = false,
                PositionalMethodParameterValues = new List<string>() { },
            };
            return step;
        }
    }
}
