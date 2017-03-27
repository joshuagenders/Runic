using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Framework.ThreadPatterns
{
    public class ConstantThreadPattern : IThreadPattern
    {
        private List<Action<int>> _callbacks { get; set; }

        public int ThreadCount { get; set; }

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
            await Task.Run(() =>
            {
                _callbacks.ForEach(c => c.Invoke(ThreadCount));
            }, ct);
        }
    }
}
