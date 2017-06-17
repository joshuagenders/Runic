using NLog;
using Runic.Agent.Data;
using Runic.Agent.FlowManagement;
using Runic.Agent.Harness;
using Runic.Agent.Metrics;
using Runic.Agent.ThreadPatterns;
using Runic.Framework.Models;
using Runic.Agent.AssemblyManagement;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Services;

namespace Runic.Agent.ThreadManagement
{
    public class ThreadOrchestrator : IThreadOrchestrator
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IMessagingService _messagingService { get; }
        private readonly IFlowManager _flowManager;
        private readonly IPluginManager _pluginManager;
        private readonly IStats _stats;
        private readonly IDataService _dataService;

        public ThreadOrchestrator(
            IPluginManager pluginManager, 
            IMessagingService messagingService, 
            IFlowManager flowManager, 
            IStats stats, 
            IDataService dataService)
        {
            _flowManager = flowManager;
            _pluginManager = pluginManager;
            _messagingService = messagingService;
            _stats = stats;
            _dataService = dataService;

            _threadManagers = new ConcurrentDictionary<string, ThreadManager>();
            _threadPatterns = new ConcurrentDictionary<string, CancellableTask>();
        }

        private static ConcurrentDictionary<string, ThreadManager> _threadManagers { get; set; }
        private static ConcurrentDictionary<string, CancellableTask> _threadPatterns { get; set; }

        public void SafeCancelAll(CancellationToken ct)
        {
            var updateTasks = new List<Task>();

            _threadPatterns.ToList().ForEach(t => t.Value.Cancel());

            _threadManagers.ToList().ForEach(ftm => updateTasks.Add(ftm.Value.SafeUpdateThreadCountAsync(0)));

            Task.WaitAll(updateTasks.ToArray(), ct);
        }

        public int GetThreadLevel(string flowId)
        {
            if (_threadManagers.TryGetValue(flowId, out ThreadManager manager))
            {
                return manager.GetCurrentThreadCount();
            }

            return 0;
        }

        public async Task GetCompletionTaskAsync(string patternExecutionId)
        {
            if (_threadPatterns.ContainsKey(patternExecutionId))
            {
                await _threadPatterns[patternExecutionId].GetCompletionTaskAsync();
            }
        }

        public IList<string> GetRunningThreadPatterns()
        {
            return _threadPatterns.Select(p => p.Key).ToList();
        }

        public int GetRunningThreadPatternCount()
        {
            return _threadPatterns.Count;
        }

        public IList<string> GetRunningFlows()
        {
            return _threadManagers.Select(t => t.Key).ToList();
        }

        public int GetRunningFlowCount()
        {
            return _threadManagers.Count;
        }

        public void StopThreadPattern(string patternExecutionId)
        {
            if (_threadPatterns.TryRemove(patternExecutionId, out CancellableTask task))
            {
                task.Cancel();
                return;
            }
        }

        public void AddNewPattern(string patternExecutionId, Flow flow, IThreadPattern pattern)
        {
            if (_threadManagers.TryGetValue(patternExecutionId, out ThreadManager manager))
            {
                //pattern already exists
                return;
            }
            else
            {
                var cts = new CancellationTokenSource();
                var patternTask = ExecutePattern(patternExecutionId, flow, pattern, cts.Token);
                //.ContinueWith(async (_) => await SafeRemoveTaskAsync(patternExecutionId));

                var cancellableTask = new CancellableTask(patternTask, cts);
                _threadPatterns.AddOrUpdate(patternExecutionId, cancellableTask,
                    (key, val) => {
                        val.Cancel();
                        return cancellableTask;
                    });
            }
        }

        private async Task ExecutePattern(string flowExecutionId, Flow flow, IThreadPattern pattern, CancellationToken ct)
        {
            _flowManager.AddUpdateFlow(flow);

            pattern.RegisterThreadChangeHandler(async (threadLevel) =>
            {
                await SetThreadLevelAsync(new SetThreadLevelRequest()
                {
                    FlowName = flow.Name,
                    ThreadLevel = threadLevel,
                    FlowId = flowExecutionId
                }, ct);
            });

            await pattern.StartPatternAsync(ct);
        }

        private async Task SafeRemoveThreadPatternAsync(string id)
        {
            CancellableTask task;
            _threadPatterns.TryRemove(id, out task);
            if (task != null)
                await task.CancelAsync();
        }

        public async Task SetThreadLevelAsync(SetThreadLevelRequest request, CancellationToken ct)
        {
            //todo implement maxthreads
            _logger.Debug($"Attempting to update thread level to {request.ThreadLevel} for {request.FlowName}");
            if (_threadManagers.TryGetValue(request.FlowId, out ThreadManager manager))
            {
                await manager.SafeUpdateThreadCountAsync(request.ThreadLevel);
            }
            else
            {
                var newThreadManager = new ThreadManager(_flowManager.GetFlow(request.FlowName), _pluginManager, _stats, _dataService);
                var resolvedManager = _threadManagers.GetOrAdd(request.FlowId, newThreadManager);
                await resolvedManager.SafeUpdateThreadCountAsync(request.ThreadLevel);
            }
        }

        public void Dispose()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(2000);
            SafeCancelAll(cts.Token);
        }

        public void StopFlow(string flowExecutionId)
        {
            if (_threadManagers.TryRemove(flowExecutionId, out ThreadManager threadManager))
            {
                threadManager.StopAll();
            }
        }

        public void StopPattern(string patternExecutionId)
        {
            if (_threadPatterns.TryRemove(patternExecutionId, out CancellableTask task))
            {
                task.Cancel();
                task.GetCompletionTaskAsync().Wait();
            }
        }
    }
}
