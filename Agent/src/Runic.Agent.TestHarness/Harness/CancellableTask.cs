using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.TestHarness.FunctionHarness
{
    public sealed class CancellableTask : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly Task _task;
        private readonly ManualResetEventSlim _mre;

        public CancellableTask(Task task, CancellationTokenSource cts)
        {
            _mre = new ManualResetEventSlim(false);
            _task = task.ContinueWith((r) =>
            {
                _mre.Set();
            });
            _cts = cts;
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

        public void Cancel()
        {
            try
            {
                _cts.Cancel();
                _task.GetAwaiter().GetResult();
            }
            catch (TaskCanceledException)
            {
                //do nothing
            }
        }

        public async Task CancelAsync()
        {
            try
            {
                _cts.Cancel();
                await _task;
            }
            catch (TaskCanceledException)
            {
                //do nothing
            }
        }

        public void Dispose()
        {
            Cancel();
        }
    }
}