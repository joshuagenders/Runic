using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.FunctionHarness
{
    public class SafeCancellationToken
    {
        public bool IsCancelled { get; set; }
    }

    public class CancellableTask : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly Task _task;
        private readonly ManualResetEventSlim _mre;
        private readonly SafeCancellationToken _safeToken;
        public SafeCancellationToken SafeToken => _safeToken;

        public CancellableTask(Task task, SafeCancellationToken safeToken, CancellationTokenSource cts)
        {
            _mre = new ManualResetEventSlim(false);
            _task = task.ContinueWith((r) =>
            {
                _mre.Set();
            });
            _cts = cts;
            _safeToken = safeToken;
        }

        public async Task GetCompletionTaskAsync()
        {
            await Task.Run(() =>
            {
                _cts.Token.Register(() => _mre.Set());
                _mre.Wait();
            }, _cts.Token);
        }

        public bool IsComplete()
        {
            if (_task == null)
                return true;

            if (_task.IsCanceled ||
                _task.IsCompleted ||
                _task.IsFaulted)
            {
                return true;
            }

            return false;
        }

        public void CancelUnsafe()
        {
            _cts.Cancel();
        }

        public void Cancel()
        {
            _cts.CancelAfter(5000);
            SafeToken.IsCancelled = true;
            _task.GetAwaiter().GetResult();
        }

        public async Task CancelAsync()
        {
            _cts.CancelAfter(5000);
            SafeToken.IsCancelled = true;
            await _task;
        }

        public void Dispose()
        {
            CancelUnsafe();
        }
    }
}
