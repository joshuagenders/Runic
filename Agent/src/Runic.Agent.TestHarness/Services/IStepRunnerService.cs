using Runic.Agent.TestHarness.StepController;
using Runic.Agent.Framework.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.TestHarness.Services
{
    public interface IStepRunnerService
    {
        Task<Result> ExecuteStepAsync(Step step, CancellationToken ctx = default(CancellationToken));
    }
}
