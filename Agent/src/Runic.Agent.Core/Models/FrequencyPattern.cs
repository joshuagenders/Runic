using System.Collections.Generic;

namespace Runic.Agent.Core.Models
{
    public enum PatternType
    {
        Constant,
        Gradual,
        Graph
    }

    public class FrequencyPattern
    {
        public FrequencyPattern(
            PatternType patternType, 
            int durationSeconds, 
            double maxJourneysPerMinute, 
            int rampUpSeconds,
            int rampDownSeconds, 
            IReadOnlyList<Point> points)
        {
            PatternType = patternType;
            DurationSeconds = durationSeconds;
            MaxJourneysPerMinute = maxJourneysPerMinute;
            RampDownSeconds = rampDownSeconds;
            RampUpSeconds = rampUpSeconds;
            Points = points;
        }

        public PatternType PatternType { get; }
        public int DurationSeconds { get; }
        public double MaxJourneysPerMinute { get; }
        //gradual
        public int RampUpSeconds { get; }
        public int RampDownSeconds { get; }
        //graph
        public IReadOnlyList<Point> Points { get; }
    }
}
