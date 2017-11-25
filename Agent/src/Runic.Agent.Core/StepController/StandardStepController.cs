using Runic.Agent.Core.Models;
using System.Linq;

namespace Runic.Agent.Core.StepController
{
    public class StandardStepController
    {
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

            if (result.Step?.Function != null &&
                result.Step.Function.GetNextStepFromMethodResult &&
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
