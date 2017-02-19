using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Harness
{
    public interface ITestController
    {
        Task BeginTest(string testName, CancellationToken ct = default(CancellationToken));
        Task EndTest(string testName, CancellationToken ct = default(CancellationToken));
    }
}