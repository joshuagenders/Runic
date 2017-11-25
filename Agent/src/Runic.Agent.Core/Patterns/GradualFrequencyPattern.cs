using System;

namespace Runic.Agent.Core.Patterns
{
    public class GradualFrequencyPattern : IFrequencyPattern
    {
        public double JourneysPerMinute { get; set; }
        public int RampUpSeconds { get; set; }
        public int RampDownSeconds { get; set; }
        public int DurationSeconds { get; set; }
        public string PatternType => "gradual";
        public double MaxJourneysPerMinute => JourneysPerMinute;
                
        public double GetCurrentFrequencyPerMinute(DateTime startTime, DateTime now)
        {
            if (now < startTime)
                return 0;

            if (DurationSeconds > 0 && now > startTime.AddSeconds(DurationSeconds))
            {
                return 0;
            }

            if (now < startTime.AddSeconds(RampUpSeconds))
            {
                return (JourneysPerMinute / RampUpSeconds) * now.Subtract(startTime).Seconds;
            }

            if (DurationSeconds > 0 && now > startTime.AddSeconds(DurationSeconds - RampDownSeconds))
            {
                double gradient = JourneysPerMinute / RampDownSeconds;
                var timeEllapsed = now.Subtract(startTime.AddSeconds(DurationSeconds - RampDownSeconds)).Seconds;
                return JourneysPerMinute - (gradient * timeEllapsed);
            }

            return JourneysPerMinute;
        }
    }
}
