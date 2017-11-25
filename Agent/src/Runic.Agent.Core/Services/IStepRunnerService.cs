using Runic.Agent.Core.StepController;
using Runic.Agent.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Services
{
    internal interface IStepRunnerService
    {
        Task<Result> ExecuteStepAsync(Step step, CancellationToken ctx = default(CancellationToken));
    }
}
