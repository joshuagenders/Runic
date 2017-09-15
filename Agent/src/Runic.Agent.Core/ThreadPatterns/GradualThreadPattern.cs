using Runic.Agent.TestHarness.Services;
using System;

namespace Runic.Agent.Core.ThreadPatterns
{
    public class GradualThreadPattern : IThreadPattern
    {
        private readonly IDatetimeService _datetimeService;

        public GradualThreadPattern(IDatetimeService datetimeService)
        {
            _datetimeService = datetimeService;
        }

        public int ThreadCount { get; set; }
        public int RampUpSeconds { get; set; }
        public int RampDownSeconds { get; set; }
        public int DurationSeconds { get; set; }

        public int GetMaxThreadCount() => ThreadCount;
        public string GetPatternType() => "gradual";

        public int GetCurrentThreadLevel(DateTime startTime)
        {
            var now = _datetimeService.Now;
            if (now > startTime.AddSeconds(DurationSeconds))
            {
                return 0;
            }
            if (now < startTime)
            {
                return 0;
            }
            if (now > startTime.AddSeconds(DurationSeconds))
            {
                return 0;
            }
            if (now < startTime.AddSeconds(RampUpSeconds))
            {
                double gradient = (double)ThreadCount / RampUpSeconds;
                var threadLevel = (int)(gradient * now.Subtract(startTime).Seconds);
                return threadLevel == 0 ? 1 : threadLevel;
            }
            if (now > startTime.AddSeconds(DurationSeconds - RampDownSeconds))
            {
                double gradient = (double)ThreadCount / RampDownSeconds;
                var timeEllapsed = now.Subtract(startTime.AddSeconds(DurationSeconds - RampDownSeconds)).Seconds;
                var threadLevel = ThreadCount - (int)(gradient * timeEllapsed);
                return threadLevel == 0 ? 1 : threadLevel;
            }

            return ThreadCount;
        }

        public int GetMaxDurationSeconds() => DurationSeconds;
    }
}
