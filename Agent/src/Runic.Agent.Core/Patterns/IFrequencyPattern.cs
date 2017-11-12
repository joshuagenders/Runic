using System;

namespace Runic.Agent.Core.Patterns
{
    public interface IFrequencyPattern
    {
        double GetCurrentFrequencyPerMinute(DateTime startTime, DateTime now);
        int DurationSeconds { get; }
        double MaxJourneysPerMinute { get; }
        string PatternType { get; }
    }
}
