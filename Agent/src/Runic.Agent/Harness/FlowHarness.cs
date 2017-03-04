using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Core.Models;

namespace Runic.Agent.Harness
{
    public class FlowHarness
    {
        private object _instance { get; set; }
        private ThreadControl _threadControl { get; set; }
        private List<CancellationTokenSource> _cancellationSources { get; set; }
        private Flow _flow { get; set; }

        public async Task Execute(Flow flow, ThreadControl threadControl, CancellationToken ct)
        {
            _threadControl = threadControl;
            _cancellationSources = new List<CancellationTokenSource>();
            _flow = flow;

            InitialiseFunction();

            var trackedTasks = new List<Task>();

            while (!ct.IsCancellationRequested)
            {
                trackedTasks.Add(ExecuteFlow());
            }
            await Task.WhenAll(trackedTasks);

            _cancellationSources.ForEach(c => c.Cancel());
        }

        private async Task ExecuteFlow()
        {
            var cts = new CancellationTokenSource();
            _cancellationSources.Add(cts);
            await _threadControl.BeginTest(cts.Token);
            foreach (var step in _flow.Steps)
            {
                //load the library if needed

                //instantiate the class if needed

                //execute with function harness

                //move next or move start
            }
            
        }

        private void InitialiseFunction()
        {
            
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
