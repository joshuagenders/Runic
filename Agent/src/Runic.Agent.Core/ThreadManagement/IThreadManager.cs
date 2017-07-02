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
        Task SafeCancelAll();
        Task SetThreadLevelAsync(SetThreadLevelRequest request, CancellationToken ct);
        IList<string> GetRunningFlows();
        int GetRunningFlowCount();
        bool FlowExists(string flowExecutionId);
    }
}
