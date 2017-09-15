using Runic.Agent.TestHarness.Services;
using System;
using System.Collections.Generic;

namespace Runic.Agent.Core.ThreadPatterns
{
    public class ConstantThreadPattern : IThreadPattern
    {
        private List<Tuple<DateTime,int>> _threadEvents { get; set; }
        private readonly IDatetimeService _datetimeService;

        public int ThreadCount { get; set; }
        public int DurationSeconds { get; set; }

        public ConstantThreadPattern(IDatetimeService datetimeService)
        {
            _threadEvents = new List<Tuple<DateTime, int>>();
            _datetimeService = datetimeService;
        }

        public int GetMaxDurationSeconds() => DurationSeconds;
        public int GetMaxThreadCount() => ThreadCount;

        public int GetCurrentThreadLevel(DateTime startTime)
        {
            if (DurationSeconds > 0 && _datetimeService.Now > startTime.AddSeconds(DurationSeconds))
            {
                return 0;
            }
            if (_datetimeService.Now < startTime)
            {
                return 0;
            }
            return ThreadCount;
        }

        public string GetPatternType() => "constant";
    }
}
