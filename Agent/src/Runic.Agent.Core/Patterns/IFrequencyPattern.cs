using System;

namespace Runic.Agent.Core.Patterns
{
    public interface IFrequencyPattern
    {
        double GetCurrentFrequencyPerMinute(DateTime startTime, DateTime now);
        int DurationSeconds { get; set;  }
        double MaxJourneysPerMinute { get; }
        string PatternType { get; }
    }
}
