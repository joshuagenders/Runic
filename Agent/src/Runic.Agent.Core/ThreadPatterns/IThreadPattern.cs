using System;

namespace Runic.Agent.Core.ThreadPatterns
{
    public interface IThreadPattern
    {
        int GetCurrentThreadLevel(DateTime startTime);
        int GetMaxDurationSeconds();
        int GetMaxThreadCount();
        string GetPatternType();
    }
}
