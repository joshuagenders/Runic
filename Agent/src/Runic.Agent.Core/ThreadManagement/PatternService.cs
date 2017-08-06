using Microsoft.Extensions.Logging;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Framework.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using Runic.Framework.Clients;

namespace Runic.Agent.Core.ThreadManagement
{
    public class PatternService : IPatternService
    {
        private readonly ILogger _logger;
        private readonly IStatsClient _stats;
        private readonly IThreadManager _threadManager;
        private readonly IFlowManager _flowManager;
        
        private static ConcurrentDictionary<string, CancellableTask> _threadPatterns { get; set; }

        public PatternService(IFlowManager flowManager, IStatsClient stats, IThreadManager IThreadManager, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PatternService>();
            _threadManager = IThreadManager;
            _stats = stats;
            _flowManager = flowManager;
            _threadPatterns = new ConcurrentDictionary<string, CancellableTask>();
        }
        
        public void StartThreadPattern(string flowExecutionId, Flow flow, IThreadPattern pattern, CancellationToken ctx = default(CancellationToken))
        {
            if (_threadManager.FlowExists(flowExecutionId))
            {
                //pattern already exists
                return;
            }
            var cts = new CancellationTokenSource();
            var patternTask = ExecutePatternAsync(flowExecutionId, flow, pattern, cts.Token);
            var cancellableTask = new CancellableTask(patternTask, cts);

            _threadPatterns.AddOrUpdate(flowExecutionId, cancellableTask,
                (key, val) => {
                    val.Cancel();
                    return cancellableTask;
                });
        }

        public async Task SafeCancelAllPatternsAsync(CancellationToken ctx = default(CancellationToken))
        {
            var updateTasks = new List<Task>();

            _threadPatterns.ToList().ForEach(t => t.Value.Cancel());
            await _threadManager.CancelAll();
            await Task.WhenAll(updateTasks.ToArray());
        }

        public async Task StopThreadPatternAsync(string flowExecutionId, CancellationToken ctx = default(CancellationToken))
        {
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

        private async Task SafeRemoveThreadPatternAsync(string id)
        {
            CancellableTask task;
            _threadPatterns.TryRemove(id, out task);
            if (task != null)
                await task.CancelAsync();
        }

        private async Task ExecutePatternAsync(string flowExecutionId, Flow flow, IThreadPattern pattern, CancellationToken ctx = default(CancellationToken))
        {
            _flowManager.AddUpdateFlow(flow);

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
            SafeCancelAllPatternsAsync(cts.Token).Wait();
        }

        public async Task SetThreadLevelAsync(SetThreadLevelRequest request, CancellationToken ctx = default(CancellationToken))
        {
            await _threadManager.SetThreadLevelAsync(request, ctx);
        }
    }
}
