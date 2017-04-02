using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Models;
using System.Collections.Generic;

namespace Runic.Agent.Service
{
    public interface IAgentService
    {
        Task Run(CancellationToken ct);
        Task SetThreadLevel(SetThreadLevelRequest request, CancellationToken ct);
        int GetThreadLevel(string flowId);
        IList<string> GetRunningFlows();
        IList<string> GetRunningThreadPatterns();
        void SafeCancelAll();
    }
}
