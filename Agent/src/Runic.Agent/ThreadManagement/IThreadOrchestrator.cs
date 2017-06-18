using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Models;
using Runic.Agent.ThreadPatterns;
using System.Collections.Generic;

namespace Runic.Agent.ThreadManagement
{
    public interface IThreadOrchestrator
    {
        IList<string> GetRunningThreadPatterns { get; }
        int GetRunningThreadPatternCount { get; }
        IList<string> GetRunningFlows { get; }
        int GetRunningFlowCount { get; }

        Task SetThreadLevelAsync(SetThreadLevelRequest request, CancellationToken ct);
        void SafeCancelAll(CancellationToken ct);
        void StopFlow(string flowExecutionId);
        void StopThreadPattern(string flowExecutionId);
        void AddNewPattern(string patternExecutionId, Flow flow, IThreadPattern pattern);
    }
}
