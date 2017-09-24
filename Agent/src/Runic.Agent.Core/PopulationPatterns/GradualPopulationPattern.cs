using Runic.Agent.TestHarness.Services;
using System;

namespace Runic.Agent.Core.ThreadPatterns
{
    public class GradualPopulationPattern : IPopulationPattern
    {
        private readonly IDatetimeService _datetimeService;
        public int PersonCount { get; set; }
        public int RampUpSeconds { get; set; }
        public int RampDownSeconds { get; set; }
        public int DurationSeconds { get; set; }

        public int GetMaxPersonCount() => PersonCount;
        public int GetMaxDurationSeconds() => DurationSeconds;
        public string GetPatternType() => "gradual";

        public GradualPopulationPattern(IDatetimeService datetimeService)
        {
            _datetimeService = datetimeService;
        }
        
        public int GetCurrentActivePopulationCount(DateTime startTime)
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
                double gradient = (double)PersonCount / RampUpSeconds;
                var threadLevel = (int)(gradient * now.Subtract(startTime).Seconds);
                return threadLevel == 0 ? 1 : threadLevel;
            }
            if (now > startTime.AddSeconds(DurationSeconds - RampDownSeconds))
            {
                double gradient = (double)PersonCount / RampDownSeconds;
                var timeEllapsed = now.Subtract(startTime.AddSeconds(DurationSeconds - RampDownSeconds)).Seconds;
                var threadLevel = PersonCount - (int)(gradient * timeEllapsed);
                return threadLevel == 0 ? 1 : threadLevel;
            }

            return PersonCount;
        }
    }
}
