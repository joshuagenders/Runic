using System.Threading;
using System.Threading.Tasks;
using Runic.Core.Models;

namespace Runic.Agent.Harness
{
    public interface IFlowHarness
    {
        Task Execute(Flow flow, ThreadControl threadControl, CancellationToken ctx = default(CancellationToken));
        Task UpdateThreads(int threadCount, CancellationToken ctx = default(CancellationToken));
    }
}
