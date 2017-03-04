using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NLog;
using Runic.Agent.AssemblyManagement;
using Runic.Core.Attributes;
using Runic.Core.Models;

namespace Runic.Agent.Harness
{
    public class FlowHarness
    {
        private Dictionary<string,object> _instances { get; set; }
        private ThreadControl _threadControl { get; set; }
        private List<CancellationTokenSource> _cancellationSources { get; set; }
        private Flow _flow { get; set; }
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task Execute(Flow flow, ThreadControl threadControl, CancellationToken ct)
        {
            _logger.Info($"Executing flow {flow.Name}");
            _threadControl = threadControl;
            _cancellationSources = new List<CancellationTokenSource>();
            _flow = flow;

            var trackedTasks = new List<Task>();

            while (!ct.IsCancellationRequested)
            {
                trackedTasks.Add(ExecuteFlow(ct));
            }
            await Task.WhenAll(trackedTasks);

            _cancellationSources.ForEach(c => c.Cancel());
        }

        private async Task ExecuteFlow(CancellationToken ct)
        {
            var cts = new CancellationTokenSource();
            _cancellationSources.Add(cts);
            await _threadControl.BeginTest(cts.Token);
            while (!ct.IsCancellationRequested)
            {
                //todo handle complex flows
                foreach (var step in _flow.Steps)
                {
                    //load the library if needed
                    LoadLibrary(step);
                    //instantiate the class if needed
                    InitialiseFunction(step);
                    //execute with function harness
                    var instance = _instances[step.TestName];
                    instance
                        .GetType()
                        .GetMethods()
                        .Single(m => m.GetCustomAttribute<FunctionAttribute>()?.Name == step.TestName)
                        .Invoke(instance, null);
                }
            }
        }

        private void LoadLibrary(Step step)
        {
            PluginManager.LoadPlugin(step.TestAssemblyName);
        }

        private void InitialiseFunction(Step step)
        {
            if (_instances.ContainsKey(step.TestName))
                return;

            var type = PluginManager.GetTestType(step.TestName);
            _instances.Add(step.TestName, Activator.CreateInstance(type, null));
        }

        private void CancelAllThreads()
        {
            _cancellationSources.ForEach(t =>t.Cancel());   
        }

        public async void UpdateThreads(int threadCount)
        {
            CancelAllThreads();
            await _threadControl.UpdateThreadCount(threadCount);
        }
    }

}
