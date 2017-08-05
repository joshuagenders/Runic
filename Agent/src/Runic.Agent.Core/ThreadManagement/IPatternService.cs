using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Models;
using Runic.Agent.Core.ThreadPatterns;
using System.Collections.Generic;

namespace Runic.Agent.Core.ThreadManagement
{
    public interface IPatternService
    {
        Task SafeCancelAllPatternsAsync(CancellationToken ctx = default(CancellationToken));
        Task StopThreadPatternAsync(string flowExecutionId, CancellationToken ctx = default(CancellationToken));
        void StartThreadPattern(string flowExecutionId, Flow flow, IThreadPattern pattern, CancellationToken ctx = default(CancellationToken));
        Task GetCompletionTaskAsync(string flowExecutionId, CancellationToken ctx = default(CancellationToken));
        Task SetThreadLevelAsync(SetThreadLevelRequest request, CancellationToken ctx = default(CancellationToken));

        IList<string> GetRunningThreadPatterns();
        int GetRunningThreadPatternCount();
    }
}
