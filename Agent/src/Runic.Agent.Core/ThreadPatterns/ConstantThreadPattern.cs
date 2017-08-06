using Runic.Agent.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadPatterns
{
    public class ConstantThreadPattern : IThreadPattern
    {
        private List<Action<int>> _callbacks { get; set; }
        private readonly IDatetimeService _datetimeService;

        public int ThreadCount { get; set; }
        public int DurationSeconds { get; set; }

        public ConstantThreadPattern(IDatetimeService datetimeService)
        {
            _callbacks = new List<Action<int>>();
            _datetimeService = datetimeService;
        }

        public void RegisterThreadChangeHandler(Action<int> callback) => _callbacks.Add(callback);
        public int GetMaxDurationSeconds() => DurationSeconds;
        public int GetMaxThreadCount() => ThreadCount;

        public async Task StartPatternAsync(CancellationToken ctx = default(CancellationToken))
        {
            if (DurationSeconds == 0)
            {
                _callbacks.ForEach(c => c.Invoke(ThreadCount));
            }
            else
            {
                _callbacks.ForEach(c => c.Invoke(ThreadCount));
                await _datetimeService.WaitUntil(DurationSeconds, ctx);
                _callbacks.ForEach(c => c.Invoke(0));
            }
        }
    }
}
