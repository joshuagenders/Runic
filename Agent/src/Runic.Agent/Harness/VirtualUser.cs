using System;
using System.Threading;
using Runic.Agent.AssemblyManagement;

namespace Runic.Agent.Harness
{
    public class VirtualUser : IDisposable
    {
        private readonly TestOptions _options;

        public VirtualUser(TestOptions options, string testType)
        {
            _harness = new TestHarness(PluginManager.GetTestType(testType));
            _options = options;
        }

        private TestHarness _harness { get; }
        private CancellationToken _ct { get; set; }
        private ManualResetEvent _completionEvent { get; set; }

        public void Dispose()
        {
            StopThread();
        }

        public async void StartThread(CancellationToken ct, ManualResetEvent completionEvent)
        {
            _ct = ct;
            _completionEvent = completionEvent;
            _completionEvent.Reset();

            while (!ct.IsCancellationRequested)
                try
                {
                    Thread.Sleep(_options.StepDelayMilliseconds);
                    await _harness.Execute();
                }
                catch
                {
                    // TODO log
                }
            StopThread();
        }

        public async void StopThread()
        {
            _completionEvent.Set();
            await _harness.TeardownTest();
            await _harness.TeardownClass();
        }
    }
}