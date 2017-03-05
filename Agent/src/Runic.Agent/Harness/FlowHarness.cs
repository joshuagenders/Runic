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
        private ThreadControl _threadControl { get; set; }
        private List<CancellationTokenSource> _cancellationSources { get; set; }
        private Flow _flow { get; set; }
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private ConcurrentBag<Task> _trackedTasks { get; set; }

        public async Task Execute(Flow flow, ThreadControl threadControl, CancellationToken ctx = default(CancellationToken))
        {
            _logger.Info($"Executing flow {flow.Name}");
            _threadControl = threadControl;
            _cancellationSources = new List<CancellationTokenSource>();
            _flow = flow;
            _trackedTasks = new ConcurrentBag<Task>();

            while (!ctx.IsCancellationRequested)
            {
                await _threadControl.BeginTest(ctx);
                _logger.Info($"Starting thread for {flow.Name}");
                var cts = new CancellationTokenSource();
                _cancellationSources.Add(cts);
                _trackedTasks.Add(
                    ExecuteFlow(cts.Token).ContinueWith((_) =>
                    {
                        cts.Cancel();
                        _cancellationSources.Remove(cts);
                    }, ctx));
            }
            _cancellationSources.ForEach(c => c.Cancel());
            await Task.WhenAll(_trackedTasks);
            _logger.Info($"Completed flow execution for {flow.Name}");
        }

        public int GetRunningThreadCount()
        {
            return _trackedTasks.Count(t => t.Status == TaskStatus.Running);
        }

        public Flow GetRunningFlow()
        {
            return _flow;
        }

        private async Task ExecuteFlow(CancellationToken ctx = default(CancellationToken))
        {
            while (!ctx.IsCancellationRequested)
            {
                //todo handle complex flows
                foreach (var step in _flow.Steps)
                {
                    //load the library if needed
                    LoadLibrary(step);
                    //instantiate the class if needed
                    InitialiseFunction(step);
                    //execute with function harness
                    var instance = _instances[step.FunctionName];
                    var functionHarness = Program.Container.Resolve<IFunctionHarness>();
                    functionHarness.Bind(instance);
                    _logger.Info($"Executing step for {_flow.Name}");
                    await functionHarness.Execute(step.FunctionName, ctx);
                }
            }
        }

        public async Task UpdateThreads(int threadCount, CancellationToken ctx = default(CancellationToken))
        {
            _logger.Info($"Updating threads for {_flow.Name} from {_threadControl.GetThreadCount()} to {threadCount}");
            CancelAllThreads();
            await _threadControl.UpdateThreadCount(threadCount, ctx);
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

            var type = PluginManager.GetFunctionType(step.FunctionName);
            _instances.Add(step.FunctionName, Activator.CreateInstance(type, null));
        }

        private void CancelAllThreads()
        {
            _cancellationSources.ForEach(t => t.Cancel());   
        }
    }

}
