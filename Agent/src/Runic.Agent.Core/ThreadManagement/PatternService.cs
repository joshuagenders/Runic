﻿using Microsoft.Extensions.Logging;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Metrics;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Framework.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadManagement
{
    public class PatternService : IPatternService
    {
        private static readonly ILogger _logger = new LoggerFactory().CreateLogger(nameof(PatternService));

        private readonly IStats _stats;
        private readonly ExecutionContext _context;
        private readonly ThreadManager _threadManager;

        private static ConcurrentDictionary<string, CancellableTask> _threadPatterns { get; set; }

        public PatternService(ExecutionContext context, ThreadManager threadManager)
        {
            _context = context;
            _threadManager = threadManager;
            _stats = context.stats;
            _threadPatterns = new ConcurrentDictionary<string, CancellableTask>();
        }
        
        public void StartThreadPattern(string flowExecutionId, Flow flow, IThreadPattern pattern, CancellationToken ct)
        {
            if (_threadManager.FlowExists(flowExecutionId))
            {
                //pattern already exists
                return;
            }
            var cts = new CancellationTokenSource();
            var patternTask = ExecutePatternAsync(flowExecutionId, flow, pattern, cts.Token);
            //.ContinueWith(async (_) => await SafeRemoveTaskAsync(patternExecutionId));

            var cancellableTask = new CancellableTask(patternTask, cts);
            _threadPatterns.AddOrUpdate(flowExecutionId, cancellableTask,
                (key, val) => {
                    val.Cancel();
                    return cancellableTask;
                });
        }

        public async Task SafeCancelAllPatternsAsync(CancellationToken ct)
        {
            var updateTasks = new List<Task>();

            _threadPatterns.ToList().ForEach(t => t.Value.Cancel());
            await _threadManager.SafeCancelAll();
            await Task.WhenAll(updateTasks.ToArray());
        }

        public async Task StopThreadPatternAsync(string flowExecutionId, CancellationToken ct)
        {
            if (_threadPatterns.TryRemove(flowExecutionId, out CancellableTask task))
            {
                task.Cancel();
                await task.GetCompletionTaskAsync();
            }
        }

        public async Task GetCompletionTaskAsync(string flowExecutionId, CancellationToken ct)
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


        private async Task ExecutePatternAsync(string flowExecutionId, Flow flow, IThreadPattern pattern, CancellationToken ct)
        {
            _context.flowManager.AddUpdateFlow(flow);

            pattern.RegisterThreadChangeHandler(async (threadLevel) =>
            {
                await _threadManager.SetThreadLevelAsync(new SetThreadLevelRequest()
                {
                    FlowName = flow.Name,
                    ThreadLevel = threadLevel,
                    FlowId = flowExecutionId
                }, ct);
            });

            await pattern.StartPatternAsync(ct);
        }
        
        public void Dispose()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(2000);
            SafeCancelAllPatternsAsync(cts.Token).Wait();
        }
    }
}
