using Runic.Framework.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadManagement
{
    public interface IThreadManager
    {
        int GetThreadLevel(string flowId);
        void StopFlow(string flowExecutionId);
        Task SafeCancelAll(CancellationToken ctx = default(CancellationToken));
        Task SetThreadLevelAsync(SetThreadLevelRequest request, CancellationToken ctx = default(CancellationToken));
        IList<string> GetRunningFlows();
        int GetRunningFlowCount();
        bool FlowExists(string flowExecutionId);
    }
}
