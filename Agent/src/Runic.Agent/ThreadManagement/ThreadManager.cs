﻿using Runic.Agent.AssemblyManagement;
using Runic.Agent.Data;
using Runic.Agent.Harness;
using Runic.Agent.Metrics;
using Runic.Framework.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.ThreadManagement
{
    public class ThreadManager : IDisposable
    {
        private readonly Flow _flow;
        private readonly IPluginManager _pluginManager;
        private readonly ConcurrentExclusiveSchedulerPair _scheduler;
        private readonly TaskFactory _exclusiveTaskFactory;
        private readonly IStats _stats;
        private readonly IDataService _dataService;
        public readonly string Id;

        private ConcurrentDictionary<int, CancellableTask> _taskPool { get; set; }
        private int _currentThreadCount { get; set; }

        public ThreadManager(Flow flow, IPluginManager pluginManager, IStats stats, IDataService dataService)
        {
            Id = Guid.NewGuid().ToString("N");

            _flow = flow;
            _pluginManager = pluginManager;
            _stats = stats;
            _dataService = dataService;

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
                var flowTask = ExecuteFlowAsync(cts.Token).ContinueWith(async (_) => await SafeRemoveTaskAsync(id));

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

        private async Task ExecuteFlowAsync(CancellationToken ct)
        {
            var factory = new FunctionFactory(_flow, _pluginManager, _stats, _dataService);
            FunctionHarness function = null;
            bool lastStepSuccess = false;
            while (!ct.IsCancellationRequested)
            {
                function = factory.GetNextFunction(lastStepSuccess);
                lastStepSuccess = await function.OrchestrateFunctionExecutionAsync(ct);
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