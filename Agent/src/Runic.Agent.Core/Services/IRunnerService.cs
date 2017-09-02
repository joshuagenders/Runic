using Runic.Agent.Core.FunctionHarness;
using Runic.Framework.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Services
{
    public interface IRunnerService
    {
        Task ExecuteFlowAsync(Flow flow, SafeCancellationToken safeToken, CancellationToken ctx = default(CancellationToken));
    }
}
