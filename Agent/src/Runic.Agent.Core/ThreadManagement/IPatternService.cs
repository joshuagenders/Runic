using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Models;
using Runic.Agent.Core.ThreadPatterns;
using System.Collections.Generic;

namespace Runic.Agent.Core.ThreadManagement
{
    public interface IPatternService
    {
        Task SafeCancelAllPatternsAsync(CancellationToken ct);
        Task StopThreadPatternAsync(string flowExecutionId, CancellationToken ct);
        void StartThreadPattern(string flowExecutionId, Flow flow, IThreadPattern pattern, CancellationToken ct);
        Task GetCompletionTaskAsync(string flowExecutionId, CancellationToken ct);

        IList<string> GetRunningThreadPatterns();
        int GetRunningThreadPatternCount();
    }
}
