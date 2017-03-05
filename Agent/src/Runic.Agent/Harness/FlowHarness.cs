using System;
using System.Collections.Generic;
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

        public async Task Execute(Flow flow, ThreadControl threadControl, CancellationToken ctx = default(CancellationToken))
        {
            _logger.Info($"Executing flow {flow.Name}");
            _threadControl = threadControl;
            _cancellationSources = new List<CancellationTokenSource>();
            _flow = flow;

            var trackedTasks = new List<Task>();

            while (!ctx.IsCancellationRequested)
            {
                trackedTasks.Add(ExecuteFlow(ctx));
            }
            await Task.WhenAll(trackedTasks);

            _cancellationSources.ForEach(c => c.Cancel());
        }

        private async Task ExecuteFlow(CancellationToken ctx = default(CancellationToken))
        {
            var cts = new CancellationTokenSource();
            _cancellationSources.Add(cts);
            await _threadControl.BeginTest(cts.Token);
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
                    functionHarness.Execute(step.FunctionName, cts.Token);
                }
            }
        }

        public async Task UpdateThreads(int threadCount, CancellationToken ctx = default(CancellationToken))
        {
            CancelAllThreads();
            await _threadControl.UpdateThreadCount(threadCount, ctx);
        }

        private void LoadLibrary(Step step)
        {
            PluginManager.LoadPlugin(step.FunctionAssemblyName);
        }

        private void InitialiseFunction(Step step)
        {
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
