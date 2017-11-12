using System;

namespace Runic.Agent.Core.Patterns
{
    public class ConstantFrequencyPattern : IFrequencyPattern
    {
        public double JourneysPerMinute { get; set; }
        public int DurationSeconds { get; set; }
        public double MaxJourneysPerMinute => JourneysPerMinute;
        public string PatternType => "constant";

        public double GetCurrentFrequencyPerMinute(DateTime startTime, DateTime now)
        {
            if (DurationSeconds > 0 && now > startTime.AddSeconds(DurationSeconds))
            {
                return 0;
            }
            if (now < startTime)
            {
                return 0;
            }
            return JourneysPerMinute;
        }
    }
}
