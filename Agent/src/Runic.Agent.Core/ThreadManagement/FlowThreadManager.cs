using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadManagement
{
    public class FlowThreadManager : IDisposable
    {
        private readonly Flow _flow;
        private readonly TaskFactory _taskFactory;
        private readonly ILoggingHandler _log;
        private readonly IStatsClient _stats;

        private ConcurrentDictionary<int, CancellableTask> _taskPool { get; set; }
        private int _currentThreadCount { get; set; }
        public readonly string Id;
        private readonly IRunnerService _runnerService;
        
        public FlowThreadManager(Flow flow, IStatsClient stats, IRunnerService runnerService, ILoggingHandler loggingHandler)
        {
            Id = Guid.NewGuid().ToString("N");
            _log = loggingHandler;
            _taskFactory = new TaskFactory(new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler);
            _taskPool = new ConcurrentDictionary<int, CancellableTask>();
            _flow = flow;
            _runnerService = runnerService;
            _stats = stats;
        }

        public int GetCurrentThreadCount()
        {
            return _currentThreadCount;
        }

        private void RequestNewThread(int id)
        {
            _log.Info($"New thread requested, id {id}");
            CancellableTask task;
            _taskPool.TryGetValue(id, out task);
            if (task != null)
            {
                return;
            }
            else
            {
                var cts = new CancellationTokenSource();
                var safeToken = new SafeCancellationToken();
                var flowTask = _runnerService.ExecuteFlowAsync(_flow, safeToken, cts.Token)
                                             .ContinueWith(async (_) => await RemoveTaskAsync(id));

                var cancellableTask = new CancellableTask(flowTask, safeToken, cts);
                _taskPool.AddOrUpdate(id, cancellableTask,
                    (key, val) => {
                        val.Cancel();
                        return cancellableTask;
                    });
            }
        }

        private void RemoveTask(int id)
        {
            _log.Info($"Remove task requested, id {id}");
            CancellableTask task;
            _taskPool.TryRemove(id, out task);
            if (task != null)
                task.Cancel();
        }

        private async Task RemoveTaskAsync(int id)
        {
            _log.Info($"Remove task requested, id {id}");
            CancellableTask task;
            _taskPool.TryRemove(id, out task);
            if (task != null)
                await task.CancelAsync();
        }

        public async Task UpdateThreadCountAsync (int threadCount)
        {
            _log.Info($"Update thread count to {threadCount} for flow {_flow.Name}");
            await _taskFactory.StartNew(() => UpdateThreads(threadCount));
        }

        public void StopAll()
        {
            UpdateThreads(0);
        }

        private void UpdateThreads(int threadCount)
        {
            if (threadCount > _currentThreadCount)
            {
                for (int i = _currentThreadCount + 1; i <= threadCount; i++)
                {
                    RequestNewThread(i);
                }
            }
            if (threadCount < _currentThreadCount)
            {
                var removalTasks = new List<Task>();
                for (int index = _currentThreadCount; index > threadCount; index--)
                {
                    removalTasks.Add(RemoveTaskAsync(index));
                }
                Task.WaitAll(removalTasks.ToArray());
            }
            _currentThreadCount = threadCount;
            _stats.SetThreadLevel(_flow.Name, threadCount);
        }

        public void Dispose()
        {
            UpdateThreads(0);
            if (_flow != null)
                _stats.SetThreadLevel(_flow.Name, 0);
        }
    }
}
