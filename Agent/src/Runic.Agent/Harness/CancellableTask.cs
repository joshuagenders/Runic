﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Harness
{
    public class CancellableTask : IDisposable
    {
        private CancellationTokenSource _cts { get; set; }
        private Task _task { get; set; }
        private ManualResetEventSlim _mre { get; set; }

        public CancellableTask(Task task, CancellationTokenSource cts)
        {
            _mre = new ManualResetEventSlim(false);
            _task = task.ContinueWith((r) =>
            {
                _mre.Set();
            });
            _cts = cts;
        }

        public async Task GetCompletionTask()
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
