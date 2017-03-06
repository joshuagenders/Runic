using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NLog;
using Runic.Agent.AssemblyManagement;
using Runic.Core.Models;

namespace Runic.Agent.Harness
{
    public class FlowHarness : IFlowHarness
    {
        private Dictionary<string,object> _instances { get; set; }
        private List<CancellationTokenSource> _cancellationSources { get; set; }
        private Flow _flow { get; set; }
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private ConcurrentBag<Task> _trackedTasks { get; set; }
        private int _threadCount { get; set; }
        private SemaphoreSlim _semaphore { get; set; }

        public void InstantiateCollections()
        {
            _instances = new Dictionary<string, object>();
            _cancellationSources = new List<CancellationTokenSource>();
            _trackedTasks = new ConcurrentBag<Task>();
        }

        public async Task Execute(Flow flow, int threadCount, CancellationToken ctx = default(CancellationToken))
        {
            _logger.Info($"Executing flow {flow.Name}");
            InstantiateCollections();

            _flow = flow;
            _threadCount = threadCount;
            _semaphore = new SemaphoreSlim(_threadCount, _threadCount);

            while (!ctx.IsCancellationRequested)
            {
                //get thread permission
                await _semaphore.WaitAsync(ctx);

                _logger.Info($"Starting thread for {flow.Name}");
                var cts = new CancellationTokenSource();
                _cancellationSources.Add(cts);
                _trackedTasks.Add(ExecuteFlow(cts.Token).ContinueWith((_) =>
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
            _logger.Info($"Completed flow execution for {flow.Name}");
        }

        public List<Task> GetTasks ()
        {
            return _trackedTasks.ToList();
        }

        public int GetRunningThreadCount()
        {
            return _trackedTasks.Count(t => t.Status == TaskStatus.Running);
        }

        public int GetSemaphoreCurrentCount()
        {
            return _semaphore.CurrentCount;
        }

        public Flow GetRunningFlow()
        {
            return _flow;
        }

        private async Task ExecuteFlow(CancellationToken ctx = default(CancellationToken))
        {
            Console.WriteLine("Executing flow");
            foreach (var step in _flow.Steps)
            {
                try
                {
                    //load the library if needed
                    LoadLibrary(step);
                }
                catch (Exception e)
                {
                    _logger.Error($"Encountered error initialising flow {_flow.Name}");
                    _logger.Error(e);
                }
                //instantiate the class if needed
                InitialiseFunction(step);
            }

            while (!ctx.IsCancellationRequested)
            {
                //todo handle complex flows
                foreach (var step in _flow.Steps)
                {
                    //execute with function harness
                    var instance = _instances[step.FunctionName];
                    var functionHarness = Program.Container.Resolve<IFunctionHarness>();
                    functionHarness.Bind(instance);

                    _logger.Info($"Executing step for {_flow.Name}");
                    await functionHarness.Execute(step.FunctionName, ctx);
                }
            }
        }

        private void RestartThreads(int threadCount)
        {
            CancelAllThreads();
            _threadCount = threadCount;
            _semaphore = new SemaphoreSlim(_threadCount);
        }

        public async Task UpdateThreads(int threadCount, CancellationToken ctx = default(CancellationToken))
        {
            _logger.Info($"Updating threads for {_flow.Name} from {GetRunningThreadCount()} to {threadCount}");
            //todo if < > ==
            await Task.Run(() => RestartThreads(threadCount), ctx);
        }

        private void LoadLibrary(Step step)
        {
            _logger.Info($"Attempting to load library for {step.FunctionName} in {step.FunctionAssemblyName}");
            PluginManager.LoadPlugin(step.FunctionAssemblyName);
        }

        private void InitialiseFunction(Step step)
        {
            _logger.Info($"Initialising {step.FunctionName} in {step.FunctionAssemblyName}");
            if (_instances.ContainsKey(step.FunctionName))
                return;

            _logger.Info($"Retrieving function type");

            var type = PluginManager.GetFunctionType(step.FunctionName);
            if (type == null)
                throw new FunctionTypeNotFoundException();

            _logger.Info($"type found {type.AssemblyQualifiedName}");

            _instances[step.FunctionName] = Activator.CreateInstance(type);
            _logger.Info($"{step.FunctionName} in {step.FunctionAssemblyName} initialised");
        }

        private void CancelAllThreads()
        {
            _cancellationSources.ForEach(t => t.Cancel());   
        }
    }

}
