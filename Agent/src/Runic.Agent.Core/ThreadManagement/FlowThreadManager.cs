using Microsoft.Extensions.Logging;
using Runic.Agent.Core.Harness;
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
        private readonly IStatsClient _stats;
        private readonly TaskFactory _taskFactory;
        private readonly FunctionFactory _functionFactory;
        private readonly CucumberHarness _harness;

        public readonly string Id;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        private ConcurrentDictionary<int, CancellableTask> _taskPool { get; set; }
        private int _currentThreadCount { get; set; }

        public FlowThreadManager(Flow flow, IStatsClient stats, FunctionFactory factory, CucumberHarness harness, ILoggerFactory loggerFactory)
        {
            Id = Guid.NewGuid().ToString("N");

            _flow = flow;
            _functionFactory = factory;
            _stats = stats;
            _harness = harness;
            _taskFactory = new TaskFactory(new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler);
            _taskPool = new ConcurrentDictionary<int, CancellableTask>();
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<FlowThreadManager>();
        }

        public int GetCurrentThreadCount()
        {
            return _currentThreadCount;
        }

        private void RequestNewThread(int id)
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
                var flowTask = new FlowRunner(_functionFactory, _harness, _flow, _loggerFactory, 0)
                    .ExecuteFlowAsync(cts.Token)
                    .ContinueWith(async (_) => await RemoveTaskAsync(id));

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

        private void RemoveTask(int id)
        {
            CancellableTask task;
            _taskPool.TryRemove(id, out task);
            if (task != null)
                task.Cancel();
        }

        private async Task RemoveTaskAsync(int id)
        {
            CancellableTask task;
            _taskPool.TryRemove(id, out task);
            if (task != null)
                await task.CancelAsync();
        }

        public async Task UpdateThreadCountAsync (int threadCount)
        {
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
