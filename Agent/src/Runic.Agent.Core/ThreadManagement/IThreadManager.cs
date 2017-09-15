using Runic.Agent.Framework.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadManagement
{
    public interface IThreadManager
    {
        int GetThreadLevel(string flowId);
        Task SetThreadLevelAsync(string flowId, Flow flow, int threadLevel, CancellationToken ctx = default(CancellationToken));
        void StopFlow(string flowId);
        Task CancelAll(CancellationToken ctx = default(CancellationToken));
        IList<string> GetRunningFlows();
        int GetRunningFlowCount();
        bool FlowExists(string flowId);
    }
}
