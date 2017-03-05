using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Harness
{
    public class ThreadControl
    {
        private int _threadCount { get; set; }
        private SemaphoreSlim _semaphore { get; set; }

        public ThreadControl(int threadCount)
        {
            _threadCount = threadCount;
            _semaphore = new SemaphoreSlim(_threadCount);
        }

        public async Task UpdateThreadCount(int threadCount, CancellationToken ctx = default(CancellationToken))
        {
            await Task.Run(() =>
            {
                _threadCount = threadCount;
                _semaphore?.Dispose();
                _semaphore = new SemaphoreSlim(_threadCount);
            }, ctx);
        }

        public async Task BeginTest(CancellationToken ct = default(CancellationToken))
        {
            if (_semaphore == null)
                _semaphore = new SemaphoreSlim(_threadCount);
            await _semaphore.WaitAsync(ct);
        }

        public async Task EndTest(CancellationToken ct = default(CancellationToken))
        {
            await Task.Run(() => _semaphore?.Release(), ct);
        }
    }
}