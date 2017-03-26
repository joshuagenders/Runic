using Runic.Agent.AssemblyManagement;
using Runic.Agent.Metrics;
using Runic.Framework.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Harness
{
    public class ThreadManager : IDisposable
    {
        private readonly Flow _flow;
        private readonly PluginManager _pluginManager;
        private int _currentThreadCount { get; set; }

        private ConcurrentDictionary<int, CancellableTask> _taskPool { get; set; }
        private readonly ConcurrentExclusiveSchedulerPair _scheduler;
        private readonly TaskFactory _exclusiveTaskFactory;

        public ThreadManager(Flow flow, PluginManager pluginManager)
        {
            _flow = flow;
            _pluginManager = pluginManager;
            
            _scheduler = new ConcurrentExclusiveSchedulerPair();
            _exclusiveTaskFactory = new TaskFactory(_scheduler.ExclusiveScheduler);
            _taskPool = new ConcurrentDictionary<int, CancellableTask>();
        }

        public int GetCurrentThreadCount()
        {
            return _currentThreadCount;
        }

        public void RequestNewThread(int id)
        {
            CancellableTask task;
            _taskPool.TryGetValue(id, out task);
            if (task != null)
            {
                return;
            }
            else
            {
                var cts = new CancellationTokenSource();
                var flowTask = new FlowExecutor(_flow, _pluginManager)
                                               .ExecuteFlow(cts.Token)
                                               .ContinueWith(async (_) => await SafeRemoveTaskAsync(id));

                var cancellableTask = new CancellableTask(flowTask, cts);
                    _taskPool.AddOrUpdate(
                    id, 
                    cancellableTask, 
                    (key, val) => {
                        val.Cancel();
                        return cancellableTask;
                    });
            }
        }

        public void RemoveTask(int id)
        {
            CancellableTask task;
            _taskPool.TryRemove(id, out task);
            if (task != null)
                task.Cancel();
        }

        public async Task SafeRemoveTaskAsync(int id)
        {
            CancellableTask task;
            _taskPool.TryRemove(id, out task);
            if (task != null)
                await task.CancelAsync();
        }

        public async Task SafeUpdateThreadCountAsync (int threadCount)
        {
            await _exclusiveTaskFactory.StartNew(() => UpdateThreads(threadCount));
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
                List<Task> removalTasks = new List<Task>();
                for (int index = _currentThreadCount; index > threadCount; index--)
                {
                    removalTasks.Add(SafeRemoveTaskAsync(index));
                }
                Task.WaitAll(removalTasks.ToArray());
            }
            _currentThreadCount = threadCount;
            Clients.Statsd?.Gauge($"flows.{_flow.Name}.threadLevel", threadCount);
        }

        public void Dispose()
        {
            UpdateThreads(0);
            if (_flow != null)
                Clients.Statsd?.Gauge($"flows.{_flow.Name}.threadLevel", 0);
        }
    }
}
