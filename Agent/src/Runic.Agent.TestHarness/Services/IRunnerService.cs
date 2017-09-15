using Runic.Agent.Framework.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.TestHarness.Services
{
    public interface IRunnerService
    {
        Task ExecuteFlowAsync(Flow flow, CancellationToken ctx = default(CancellationToken));
    }
}
