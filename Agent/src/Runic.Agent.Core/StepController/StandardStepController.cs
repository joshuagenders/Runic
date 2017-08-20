using Runic.Agent.Core.Exceptions;
using Runic.Framework.Models;
using System.Linq;

namespace Runic.Agent.Core.StepController
{
    public class StandardStepController : IStepController
    {
        private bool _getNextStepFromResult { get; set; }
        private int _lastStepIndex { get; set; }
        private readonly Flow _flow;

        public StandardStepController(Flow flow)
        {
            _flow = flow;
        }
        public Step GetNextStep(Result result)
        {
            //todo refactor into interface for Irunner
            Step nextStep = null;

            if (_getNextStepFromResult && result?.NextStep != null)
            {
                var matchingStep = _flow.Steps
                                        .Where(s => s.StepName == result.NextStep);
                if (!matchingStep.Any())
                {
                    throw new StepNotFoundException($"Step not found for step {result.Step.StepName}");
                }
                if (matchingStep.Count() > 1)
                {
                    throw new StepNotFoundException($"Duplicate step found for step {result.Step.StepName}");
                }
                nextStep = matchingStep.Single();
            }

            if (result == null )
            {
                nextStep = _flow.Steps.First();
                _lastStepIndex = 0;
                _getNextStepFromResult = nextStep.GetNextStepFromFunctionResult;
            }
            else if (nextStep == null)
            {
                _lastStepIndex++;
                _lastStepIndex= _lastStepIndex >= _flow.Steps.Count ? 0 : _lastStepIndex;
                nextStep = _flow.Steps[_lastStepIndex];
            }
            if (nextStep == null)
                throw new StepNotFoundException($"Step not found for step {result.Step.StepName}");

            return nextStep;
        }
    }
}
