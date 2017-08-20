using Runic.Agent.Core.Services;
using Runic.Framework.Models;

namespace Runic.Agent.Core.StepController
{
    public interface IStepController
    {
        Step GetNextStep(Result result);
    }
}
