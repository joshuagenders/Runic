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
            Harness = new TestHarness(PluginManager.GetTestType(testType));
            _options = options;
        }

        private TestHarness Harness { get; }
        private ManualResetEvent CompletionEvent { get; set; }

        public void Dispose()
        {
            StopThread();
        }

        public async void StartThread(CancellationToken ct, ManualResetEvent completionEvent)
        {
            CompletionEvent = completionEvent;
            CompletionEvent.Reset();

            while (!ct.IsCancellationRequested)
                try
                {
                    Thread.Sleep(_options.StepDelayMilliseconds);
                    await Harness.Execute();
                }
                catch
                {
                    // TODO log
                }
            StopThread();
        }

        public async void StopThread()
        {
            CompletionEvent.Set();
            await Harness.TeardownTest();
            await Harness.TeardownClass();
        }
    }
}