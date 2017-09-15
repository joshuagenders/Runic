﻿using Runic.Agent.Core.Services;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadManagement
{
    public sealed class FlowThreadManager : IDisposable
    {
        private readonly Flow _flow;
        private readonly TaskFactory _taskFactory;

        private ConcurrentDictionary<int, CancellableTask> _taskPool { get; set; }
        private int _currentThreadCount { get; set; }
        public readonly string Id;
        private readonly IRunnerService _runnerService;
        private readonly IEventService _eventService;

        public FlowThreadManager(Flow flow, IRunnerService runnerService, IEventService eventService)
        {
            Id = Guid.NewGuid().ToString("N");
            _eventService = eventService;
            _taskFactory = new TaskFactory(new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler);
            _taskPool = new ConcurrentDictionary<int, CancellableTask>();
            _flow = flow;
            _runnerService = runnerService;
        }

        public int GetCurrentThreadCount()
        {
            return _currentThreadCount;
        }

        private void RequestNewThread(int id)
        {
            _eventService.Info($"New thread requested, id {id}");
            _taskPool.TryGetValue(id, out CancellableTask task);
            if (task == null)
            {
                var cts = new CancellationTokenSource();
                var flowTask = _runnerService.ExecuteFlowAsync(_flow, cts.Token)
                                             .ContinueWith(async (_) => await RemoveTaskAsync(id));

                var cancellableTask = new CancellableTask(flowTask, cts);
                _taskPool.AddOrUpdate(id, cancellableTask,
                    (key, val) => {
                        val.Cancel();
                        return cancellableTask;
                    });
            }
        }

        private async Task RemoveTaskAsync(int id)
        {
            _eventService.Info($"Remove task requested, id {id}");
            _taskPool.TryRemove(id, out CancellableTask task);
            if (task != null)
                await task.CancelAsync();
        }

        public async Task UpdateThreadCountAsync (int threadCount)
        {
            _eventService.Info($"Update thread count to {threadCount} for flow {_flow.Name}");
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
            _eventService.OnThreadChange(_flow, threadCount);
        }

        public void Dispose()
        {
            UpdateThreads(0);
            if (_flow != null)
                _eventService.OnThreadChange(_flow, 0);
        }
    }
}
