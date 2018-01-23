using Runic.Agent.Core.Models;
using System;
using System.Linq;

namespace Runic.Agent.Core.Services
{
    public class FrequencyService
    {
        public double GetCurrentFrequencyPerMinute(
            FrequencyPattern frequencyPattern,
            DateTime startTime,
            DateTime now)
        {
            switch (frequencyPattern.PatternType)
            {
                case PatternType.Constant:
                    return GetConstantFrequencyPerMinute(frequencyPattern, startTime, now);
                case PatternType.Gradual:
                    return GetGradualFrequencyPerMinute(frequencyPattern, startTime, now);
                case PatternType.Graph:
                    return GetGraphFrequencyPerMinute(frequencyPattern, startTime, now);
                default:
                    throw new ArgumentException("Frequency pattern not recognised");
            }
        }

        public double GetGradualFrequencyPerMinute(
            FrequencyPattern frequencyPattern, 
            DateTime startTime, 
            DateTime now)
        {
            if (now < startTime)
                return 0;

            if (frequencyPattern.DurationSeconds > 0 && 
                now > startTime.AddSeconds(frequencyPattern.DurationSeconds))
            {
                return 0;
            }

            if (now < startTime.AddSeconds(frequencyPattern.RampUpSeconds))
            {
                return (frequencyPattern.MaxJourneysPerMinute / frequencyPattern.RampUpSeconds) * now.Subtract(startTime).Seconds;
            }

            if (frequencyPattern.DurationSeconds > 0 && 
                now > startTime.AddSeconds(frequencyPattern.DurationSeconds - frequencyPattern.RampDownSeconds))
            {
                double gradient = frequencyPattern.MaxJourneysPerMinute / frequencyPattern.RampDownSeconds;
                var timeEllapsed = 
                        now.Subtract(
                            startTime.AddSeconds(
                                  frequencyPattern.DurationSeconds - frequencyPattern.RampDownSeconds))
                                 .Seconds;
                return frequencyPattern.MaxJourneysPerMinute - (gradient * timeEllapsed);
            }

            return frequencyPattern.MaxJourneysPerMinute;
        }

        public double GetConstantFrequencyPerMinute(
            FrequencyPattern frequencyPattern, 
            DateTime startTime,
            DateTime now)
        {
            if (frequencyPattern.DurationSeconds < 0)
                return frequencyPattern.MaxJourneysPerMinute;

            if (frequencyPattern.DurationSeconds > 0 && now > startTime.AddSeconds(frequencyPattern.DurationSeconds))
            {
                return 0;
            }
            if (now < startTime)
            {
                return 0;
            }
            return frequencyPattern.MaxJourneysPerMinute;
        }

        public double GetGraphFrequencyPerMinute(
            FrequencyPattern frequencyPattern, 
            DateTime startTime, 
            DateTime now)
        {
            if (now > startTime.AddSeconds(frequencyPattern.DurationSeconds)
             || now < startTime)
            {
                return 0;
            }

            double maxX = frequencyPattern.Points.Max(p => p.UnitsFromStart);
            double secondsPerPoint = (frequencyPattern.DurationSeconds / maxX);
            var timeEllapsedSeconds = now.Subtract(startTime).Seconds;
            var currentPosition = timeEllapsedSeconds / secondsPerPoint;

            Point point = frequencyPattern.Points[0];
            for (var index = 1; index < frequencyPattern.Points.Count; index++)
            {
                if (point.UnitsFromStart > currentPosition)
                {
                    break;
                }
                if (currentPosition < frequencyPattern.Points[index].UnitsFromStart)
                    break;
                point = frequencyPattern.Points[index];
            }
            return point.FrequencyPerMinute;
        }
    }
}
