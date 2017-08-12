using Runic.Framework.Models;

namespace Runic.Agent.Core.Services.Interfaces
{
    public interface IStepController
    {
        Step GetNextStep(Result result);
    }
}
