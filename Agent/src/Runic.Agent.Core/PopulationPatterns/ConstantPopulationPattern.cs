using Runic.Agent.TestHarness.Services;
using System;

namespace Runic.Agent.Core.ThreadPatterns
{
    public class ConstantPopulationPattern : IPopulationPattern
    {
        private readonly IDatetimeService _datetimeService;

        public int PersonCount { get; set; }
        public int DurationSeconds { get; set; }

        public ConstantPopulationPattern(IDatetimeService datetimeService)
        {
            _datetimeService = datetimeService;
        }

        public int GetMaxDurationSeconds() => DurationSeconds;
        public int GetMaxPersonCount() => PersonCount;

        public int GetCurrentActivePopulationCount(DateTime startTime)
        {
            if (DurationSeconds > 0 && _datetimeService.Now > startTime.AddSeconds(DurationSeconds))
            {
                return 0;
            }
            if (_datetimeService.Now < startTime)
            {
                return 0;
            }
            return PersonCount;
        }

        public string GetPatternType() => "constant";
    }
}
