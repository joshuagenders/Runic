using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Runic.Agent.AssemblyManagement;
using Runic.Framework.Models;
using Runic.Agent.Metrics;

namespace Runic.Agent.Harness
{
    public class FlowHarness
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private Flow _flow { get; set; }
        private int _threadCount { get; set; }

        private SemaphoreSlim _semaphore { get; set; }
        private ConcurrentBag<Task> _trackedTasks { get; set; }
        private List<CancellationTokenSource> _cancellationSources { get; set; }

        private readonly PluginManager _pluginManager;

        public FlowHarness(PluginManager pluginManager)
        {
            InstantiateCollections();
            _pluginManager = pluginManager;
        }

        public void InstantiateCollections()
        {
            _cancellationSources = new List<CancellationTokenSource>();
            _trackedTasks = new ConcurrentBag<Task>();
        }
        
        public async Task Execute(Flow flow, int threadCount, CancellationToken ctx = default(CancellationToken))
        {
            _logger.Debug($"Executing flow {flow.Name}");
            Clients.Statsd?.Count($"flows.{flow.Name}.flowStarted");
            InstantiateCollections();

            _flow = flow;
            _threadCount = threadCount;
            _semaphore = new SemaphoreSlim(_threadCount, _threadCount);

            while (!ctx.IsCancellationRequested && threadCount > 0)
            {
                //get thread permission
                await _semaphore.WaitAsync(ctx);

                _logger.Debug($"Starting thread for {flow.Name}");
                Clients.Statsd?.Count($"flows.{flow.Name}.threadStarted");
                var cts = new CancellationTokenSource();
                _cancellationSources.Add(cts);
                
                _trackedTasks.Add(
                    new FlowExecutor(flow, _pluginManager).ExecuteFlow(cts.Token).ContinueWith((_) =>
                    {
                        if (_.Exception != null)
                            _logger.Error(_.Exception);

                        cts.Cancel();
                        _cancellationSources.Remove(cts);
                        //free thread
                        _semaphore.Release();
                    }, ctx));
            }

            _cancellationSources.ForEach(c => c.Cancel());
            await Task.WhenAll(_trackedTasks);

            _logger.Debug($"Completed flow execution for {flow.Name}");
            Clients.Statsd?.Count($"flows.{flow.Name}.flowCompleted");
        }

        public List<Task> GetTasks()
        {
            return _trackedTasks.ToList();
        }

        public int GetTotalInitiatiedThreadCount()
        {
            return _trackedTasks.Count();
        }

        public int GetRunningThreadCount()
        {
            var completedStatuses = new[]
            {
                TaskStatus.RanToCompletion,
                TaskStatus.Canceled,
                TaskStatus.Faulted
            };
            return _trackedTasks.Count(t => !completedStatuses.Contains(t.Status));
        }

        public int GetSemaphoreCurrentCount()
        {
            return _semaphore.CurrentCount;
        }

        public Flow GetRunningFlow()
        {
            return _flow;
        }

        private void RestartThreads(int threadCount)
        {
            CancelAllThreads();
            _threadCount = threadCount;
            _semaphore = new SemaphoreSlim(_threadCount);
        }

        public async Task UpdateThreads(int threadCount, CancellationToken ctx = default(CancellationToken))
        {
            _logger.Debug($"Updating threads for {_flow.Name} from {GetRunningThreadCount()} to {threadCount}");
            //todo if < > ==
            await Task.Run(() => RestartThreads(threadCount), ctx);
        }

        private void CancelAllThreads()
        {
            _cancellationSources.ForEach(t => t.Cancel());   
        }
    }

}
