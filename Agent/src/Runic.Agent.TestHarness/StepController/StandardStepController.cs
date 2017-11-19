using Runic.Agent.Framework.Models;
using System;
using System.Linq;

namespace Runic.Agent.TestHarness.StepController
{
    public class StandardStepController
    {
        private Result _lastResult { get; set; }
        private readonly Journey _journey;

        public StandardStepController(Journey journey)
        {
            _journey = journey;
        }

        public Step GetNextStep(Result result)
        {
            if (result == null)
            {
                return _journey.Steps.First();
            }

            var last = _lastResult;
            _lastResult = result;

            if (result.Step?.Function != null &&
                result.Step.Function.GetNextStepFromFunctionResult &&
                !string.IsNullOrEmpty(result.NextStep))
            {
                var matchingStep = _journey.Steps
                                           .Where(s => s.StepName == result.NextStep);
                if (!matchingStep.Any())
                {
                    throw new StepNotFoundException($"Step not found for step {result.NextStep}");
                }
                if (matchingStep.Count() > 1)
                {
                    throw new StepNotFoundException($"Duplicate step name in journey: {result.NextStep}");
                }
                return matchingStep.Single();
            }
            
            var lastIndex = 
                _journey.Steps
                        .IndexOf(
                            _journey.Steps
                                    .Single(s => s.StepName == result.Step.StepName));

            return (lastIndex + 1 >= _journey.Steps.Count || lastIndex < 0)
                    ? _journey.Steps.First() 
                    : _journey.Steps[lastIndex + 1];
        }
    }
}
