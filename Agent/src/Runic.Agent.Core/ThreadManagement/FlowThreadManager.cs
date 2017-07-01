using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Metrics;
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
        private readonly IStats _stats;
        private readonly TaskFactory _taskFactory;
        private readonly FunctionFactory _functionFactory;

        public readonly string Id;

        private ConcurrentDictionary<int, CancellableTask> _taskPool { get; set; }
        private int _currentThreadCount { get; set; }

        public FlowThreadManager(Flow flow, IStats stats, FunctionFactory factory)
        {
            Id = Guid.NewGuid().ToString("N");

            _flow = flow;
            _functionFactory = factory;
            _stats = stats;
            _taskFactory = new TaskFactory(); 
            //new TaskFactory(new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler);
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
                var flowTask = new FlowRunner(_functionFactory, _flow)
                    .ExecuteFlowAsync(cts.Token)
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
            await _taskFactory.StartNew(() => UpdateThreads(threadCount));
        }

        public void StopAll()
        {
            var completionTasks = new List<Task>();
            foreach (var thread in _taskPool)
            {
                thread.Value.Cancel();
                completionTasks.Add(thread.Value.GetCompletionTaskAsync());
            }
            Task.WaitAll(completionTasks.ToArray());
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
                    removalTasks.Add(SafeRemoveTaskAsync(index));
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
