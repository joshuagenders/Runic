using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Models;
using System.Collections.Generic;
using Runic.Agent.ThreadPatterns;

namespace Runic.Agent.ThreadManagement
{
    public interface IThreadOrchestrator
    {
        Task SetThreadLevelAsync(SetThreadLevelRequest request, CancellationToken ct);
        int GetThreadLevel(string flowId);
        IList<string> GetRunningFlows();
        int GetRunningFlowCount();
        int GetRunningThreadPatternCount();
        IList<string> GetRunningThreadPatterns();
        void SafeCancelAll(CancellationToken ct);
        void StopFlow(string flowExecutionId);
        void StopPattern(string flowExecutionId);
        void AddNewPattern(string patternExecutionId, Flow flow, IThreadPattern pattern);
    }
}
