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
        int GetRunningFlowCount();
        int GetRunningThreadPatternCount();
        IList<string> GetRunningThreadPatterns();
        void SafeCancelAll(CancellationToken ct);
        void ExecuteFlow(GradualFlowExecutionRequest request, CancellationToken ct);
        void ExecuteFlow(GraphFlowExecutionRequest request, CancellationToken ct);
        void ExecuteFlow(ConstantFlowExecutionRequest request, CancellationToken ct);
        void StopFlow(string flowExecutionId);
        void StopPattern(string flowExecutionId);
    }
}
