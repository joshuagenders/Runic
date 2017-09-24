using System;

namespace Runic.Agent.Core.ThreadPatterns
{
    public interface IPopulationPattern
    {
        int GetCurrentActivePopulationCount(DateTime startTime);
        int GetMaxDurationSeconds();
        int GetMaxPersonCount();
        string GetPatternType();
    }
}
