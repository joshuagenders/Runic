using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.ThreadPatterns
{
    public class ConstantThreadPattern : IThreadPattern
    {
        private List<Action<int>> _callbacks { get; set; }

        public int ThreadCount { get; set; }
        public int DurationSeconds { get; set; }

        public ConstantThreadPattern()
        {
            _callbacks = new List<Action<int>>();
        }

        public void RegisterThreadChangeHandler(Action<int> callback)
        {
            _callbacks.Add(callback);
        }

        public async Task Start(CancellationToken ct)
        {
            if (DurationSeconds == 0)
            {
                await Task.Run(() =>
                {
                    _callbacks.ForEach(c => c.Invoke(ThreadCount));
                }, ct);
            }
            else
            {
                _callbacks.ForEach(c => c.Invoke(ThreadCount));
                await Task.Run(() => ct.WaitHandle.WaitOne(TimeSpan.FromSeconds(DurationSeconds)), ct);
                _callbacks.ForEach(c => c.Invoke(0));
            }
        }
    }
}
