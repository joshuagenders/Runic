using Runic.Agent.TestHarness.Services;
using Runic.Agent.Framework.Models;

namespace Runic.Agent.TestHarness.StepController
{
    public interface IStepController
    {
        Step GetNextStep(Result result);
    }
}
