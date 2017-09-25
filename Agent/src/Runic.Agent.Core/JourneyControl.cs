using Runic.Agent.Core.Services;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public sealed class JourneyControl : IDisposable
    {
        private readonly TaskFactory _taskFactory;
        private ConcurrentDictionary<int, CancellableTask> _taskPool { get; set; }
        private int _currentPopulationCount { get; set; }

        public JourneyControl()
        {
            _taskFactory = new TaskFactory(new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler);
            _taskPool = new ConcurrentDictionary<int, CancellableTask>();
        }

        public int CurrentActivePopulation()
        {
            return _currentPopulationCount;
        }

        public int FindAvailableThread()
        {
            int id = -1;
            lock (_taskPool)
            {
                var ids = _taskPool.Where(t => t.Value != null)
                                   .Select(t => t.Key)
                                   .ToList();
                if (ids.Any())
                {
                    var result = Enumerable.Range(0, _currentPopulationCount).Except(ids);
                    id = result.OrderBy(r => r).First();
                }
                else
                {
                    
                    id = _currentPopulationCount++;
                }
            }
            return id;
        }

        private void RequestNewThread()
        {
            //var id = FindAvailableThread();
            //_taskPool.TryGetValue(id, out CancellableTask task);
            //if (task == null)
            //{
            //    var cts = new CancellationTokenSource();
            //    var task = _runnerService.PerformJourneyAsync(_journey, cts.Token)
            //                                 .ContinueWith(async (_) => await RemoveTaskAsync(id));
            //
            //    var cancellableTask = new CancellableTask(flowTask, cts);
            //    _taskPool.AddOrUpdate(id, cancellableTask,
            //        (key, val) => {
            //            val.Cancel();
            //            return cancellableTask;
            //        });
            //}
        }

        private async Task RemoveTaskAsync(int id)
        {
            //_eventService.Info($"Remove task requested, id {id}");
            _taskPool.TryRemove(id, out CancellableTask task);
            if (task != null)
                await task.CancelAsync();
        }

        public async Task UpdateThreadCountAsync (int threadCount)
        {
           // _eventService.Info($"Update thread count to {threadCount} for flow {_journey.Name}");
            await _taskFactory.StartNew(() => UpdateThreads(threadCount));
        }

        public void StopAll()
        {
            UpdateThreads(0);
        }

        private void UpdateThreads(int threadCount)
        {
            //if (threadCount > _currentThreadCount)
            //{
            //    for (int i = _currentThreadCount + 1; i <= threadCount; i++)
            //    {
            //        RequestNewThread(i);
            //    }
            //}
            //if (threadCount < _currentThreadCount)
            //{
            //    var removalTasks = new List<Task>();
            //    for (int index = _currentThreadCount; index > threadCount; index--)
            //    {
            //        removalTasks.Add(RemoveTaskAsync(index));
            //    }
            //    Task.WaitAll(removalTasks.ToArray());
            //}
            //_currentThreadCount = threadCount;
            //_eventService.OnThreadChange(_journey, threadCount);
        }

        public void Dispose()
        {
            //UpdateThreads(0);
            //if (_journey != null)
            //    _eventService.OnThreadChange(_journey, 0);
        }
    }
}
