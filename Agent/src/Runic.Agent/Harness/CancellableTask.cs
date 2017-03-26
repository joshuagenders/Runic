using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Harness
{
    public class CancellableTask : IDisposable
    {
        private CancellationTokenSource _cts { get; set; }
        private Task _task { get; set; }
        
        public CancellableTask(Task task, CancellationTokenSource cts)
        {
            _task = task;
            _cts = cts;
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
            _cts.Cancel();
        }

        public async Task CancelAsync()
        {
            _cts.Cancel();
            await _task;
        }

        public void Dispose()
        {
            Cancel();
        }
    }
}
