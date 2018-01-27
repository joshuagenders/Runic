using Runic.Agent.Core.Models;
using System;
using System.Linq;

namespace Runic.Agent.Core.Services
{
    public class FrequencyService
    {
        public double GetCurrentFrequencyPerMinute(
            FrequencyPattern frequencyPattern,
            int timeEllapsedSeconds)
        {
            switch (frequencyPattern.PatternType)
            {
                case PatternType.Constant:
                    return frequencyPattern.MaxJourneysPerMinute;
                case PatternType.Gradual:
                    return GetGradualFrequencyPerMinute(frequencyPattern, timeEllapsedSeconds);
                case PatternType.Graph:
                    return GetGraphFrequencyPerMinute(frequencyPattern, timeEllapsedSeconds);
                default:
                    throw new ArgumentException("Frequency pattern not recognised");
            }
        }

        public double GetGradualFrequencyPerMinute(
            FrequencyPattern frequencyPattern, 
            int timeEllapsedSeconds)
        {
            if (timeEllapsedSeconds < frequencyPattern.RampUpSeconds)
            {
                return (frequencyPattern.MaxJourneysPerMinute / frequencyPattern.RampUpSeconds) * timeEllapsedSeconds;
            }

            if (frequencyPattern.DurationSeconds > 0 &&
                timeEllapsedSeconds > frequencyPattern.DurationSeconds - frequencyPattern.RampDownSeconds)
            {
                double gradient = frequencyPattern.MaxJourneysPerMinute / frequencyPattern.RampDownSeconds;
                return frequencyPattern.MaxJourneysPerMinute - (gradient * timeEllapsedSeconds);
            }

            return frequencyPattern.MaxJourneysPerMinute;
        }

        public double GetGraphFrequencyPerMinute(
            FrequencyPattern frequencyPattern, 
            int timeEllapsedSeconds)
        {
            double maxX = frequencyPattern.Points.Max(p => p.UnitsFromStart);
            double secondsPerPoint = (frequencyPattern.DurationSeconds / maxX);
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
