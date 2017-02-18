using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent
{
    public class ConstantTestController : ITestController
    {
        private SemaphoreSlim _semaphore { get; set; }
        private int _threadCount { get; set; }

        public ConstantTestController(int threadCount)
        {
            _threadCount = threadCount;
        }

        public async Task BeginTest(string testId, CancellationToken ct = default(CancellationToken))
        {
            if (_semaphore == null) _semaphore = new SemaphoreSlim(_threadCount);
            await _semaphore.WaitAsync(ct);
        }

        public async Task EndTest(string testId, CancellationToken ct = default(CancellationToken))
        {
            await Task.Run(() => _semaphore?.Release());
        }
    }
}
