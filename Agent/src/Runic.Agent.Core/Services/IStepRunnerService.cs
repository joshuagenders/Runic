using Runic.Agent.Core.StepController;
using Runic.Framework.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Services
{
    public interface IStepRunnerService
    {
        Task<Result> ExecuteStepAsync(Step step, CancellationToken ctx = default(CancellationToken));
    }
}
