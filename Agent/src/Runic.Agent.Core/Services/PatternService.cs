using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Framework.Models;
using Runic.Framework.Clients;
using Runic.Agent.Core.ThreadManagement;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Core.ExternalInterfaces;

namespace Runic.Agent.Core.Services
{
    public class PatternService : IPatternService
    {
        private readonly ILoggingHandler _log;
        private readonly IStatsClient _stats;
        private readonly IThreadManager _threadManager;
        private readonly IFlowManager _flowManager;
        private readonly IAgentObserver _agentObserver;
        private static ConcurrentDictionary<string, CancellableTask> _threadPatterns { get; set; }

        public PatternService(IFlowManager flowManager, IStatsClient stats, IThreadManager IThreadManager, ILoggingHandler loggingHandler, IAgentObserver agentObserver)
        {
            _log = loggingHandler;
            _threadManager = IThreadManager;
            _stats = stats;
            _flowManager = flowManager;
            _agentObserver = agentObserver;
            _threadPatterns = new ConcurrentDictionary<string, CancellableTask>();
        }
        
        public void StartThreadPattern(string flowExecutionId, Flow flow, IThreadPattern pattern, CancellationToken ctx = default(CancellationToken))
        {
            _log.Info($"Executing pattern for flow {flow.Name} {flowExecutionId}");
            if (_threadManager.FlowExists(flowExecutionId))
            {
                //pattern already exists
                return;
            }
            var cts = new CancellationTokenSource();
            var patternTask = ExecutePatternAsync(flowExecutionId, flow, pattern, cts.Token);
            var cancellableTask = new CancellableTask(patternTask, cts);
            _agentObserver.Update(new PatternInformation()
            {
                PatternExecutionId = flowExecutionId,
                PatternType = pattern.GetPatternType()
            });
            _threadPatterns.AddOrUpdate(flowExecutionId, cancellableTask,
                (key, val) => {
                    val.Cancel();
                    return cancellableTask;
                });
        }

        public async Task CancelAllPatternsAsync(CancellationToken ctx = default(CancellationToken))
        {
            _log.Info($"Cancelling all patterns");
            var updateTasks = new List<Task>();

            _threadPatterns.ToList().ForEach(t => t.Value.Cancel());
            await _threadManager.CancelAll();
            await Task.WhenAll(updateTasks.ToArray());
        }

        public async Task StopThreadPatternAsync(string flowExecutionId, CancellationToken ctx = default(CancellationToken))
        {
            _log.Info($"Stopping pattern for flow {flowExecutionId}");
            if (_threadPatterns.TryRemove(flowExecutionId, out CancellableTask task))
            {
                task.Cancel();
                await task.GetCompletionTaskAsync();
            }
        }

        public async Task GetCompletionTaskAsync(string flowExecutionId, CancellationToken ctx = default(CancellationToken))
        {
            if (_threadPatterns.ContainsKey(flowExecutionId))
            {
                await _threadPatterns[flowExecutionId].GetCompletionTaskAsync();
            }
        }

        public IList<string> GetRunningThreadPatterns ()  => _threadPatterns.Select(p => p.Key).ToList();
        public int GetRunningThreadPatternCount () => _threadPatterns.Count;

        private async Task RemoveThreadPatternAsync(string id)
        {
            _log.Info($"Removing Thread pattern with id {id}");
            CancellableTask task;
            _threadPatterns.TryRemove(id, out task);
            if (task != null)
                await task.CancelAsync();
        }

        private async Task ExecutePatternAsync(string flowExecutionId, Flow flow, IThreadPattern pattern, CancellationToken ctx = default(CancellationToken))
        {
            _log.Info($"Executing pattern for flow {flow.Name} {flowExecutionId}");

            pattern.RegisterThreadChangeHandler(async (threadLevel) =>
            {
                await _threadManager.SetThreadLevelAsync(new SetThreadLevelRequest()
                {
                    FlowName = flow.Name,
                    ThreadLevel = threadLevel,
                    FlowId = flowExecutionId
                }, ctx);
            });

            await pattern.StartPatternAsync(ctx);
        }
        
        public void Dispose()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(2000);
            CancelAllPatternsAsync(cts.Token).Wait();
        }

        public async Task SetThreadLevelAsync(SetThreadLevelRequest request, CancellationToken ctx = default(CancellationToken))
        {
            _log.Info($"Setting thread level of flow {request.FlowName} {request.FlowId} to {request.ThreadLevel}");
            await _threadManager.SetThreadLevelAsync(request, ctx);
            _agentObserver.Update(
                new FlowInformation()
                {
                    FlowExecutionId = request.FlowId,
                    ThreadCount = request.ThreadLevel
                });
        }
    }
}
