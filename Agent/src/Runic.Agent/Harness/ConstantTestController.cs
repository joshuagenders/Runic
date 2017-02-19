using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Harness
{
    public class ConstantTestController : ITestController
    {
        public ConstantTestController(int threadCount)
        {
            ThreadCount = threadCount;
        }

        private SemaphoreSlim Semaphore { get; set; }
        private int ThreadCount { get; }

        public async Task BeginTest(string testId, CancellationToken ct = default(CancellationToken))
        {
            if (Semaphore == null) Semaphore = new SemaphoreSlim(ThreadCount);
            await Semaphore.WaitAsync(ct);
        }

        public async Task EndTest(string testId, CancellationToken ct = default(CancellationToken))
        {
            await Task.Run(() => Semaphore?.Release());
        }
    }
}