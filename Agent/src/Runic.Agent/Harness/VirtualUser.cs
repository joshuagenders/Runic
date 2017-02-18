using Runic.Agent.AssemblyManagement;
using System;
using System.Threading;

namespace Runic.Agent.Harness
{
    public class VirtualUser : IDisposable
    {
        private TestHarness _harness { get; set; }
        private CancellationToken _ct { get; set; }
        private ManualResetEvent _completionEvent { get; set; }
        private TestOptions _options { get; set; }

        public VirtualUser(TestOptions options, string testType)
        {
            _harness = new TestHarness(PluginManager.GetTestType(testType));
            _options = options;
        }

        public async void StartThread(CancellationToken ct, ManualResetEvent completionEvent)
        {
            _ct = ct;
            _completionEvent = completionEvent;
            _completionEvent.Reset();
            
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    Thread.Sleep(_options.StepDelayMilliseconds);
                    await _harness.Execute();
                }
                catch
                {
                    // TODO log
                }
            }
            StopThread();
            
        }

        public async void StopThread()
        {
            _completionEvent.Set();
            await _harness.TeardownTest();
            await _harness.TeardownClass();
        }

        public void Dispose()
        {
            StopThread();
        }
    }
}
