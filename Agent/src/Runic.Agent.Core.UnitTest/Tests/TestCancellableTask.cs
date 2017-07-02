using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.Harness;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestCancellableTask
    {
        [TestMethod]
        public void CancellableTask_PollingSleepTaskCancels()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(10000);
            var cancellableTask = new CancellableTask(Task.Run(async () => await Poll(cts.Token), cts.Token), cts);
            try
            {
                cancellableTask.Cancel();
                Thread.Sleep(50);
                Assert.IsTrue(cancellableTask.IsComplete());
            }
            catch (TaskCanceledException)
            {
                //all g
            }
        }

        [TestMethod]
        public async Task CancellableTask_PollingSleepTaskCancelsAsync()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(10000);
            try
            {
                var cancellableTask = new CancellableTask(Task.Run(async () => await Poll(cts.Token), cts.Token), cts);
                await cancellableTask.CancelAsync();
                Assert.IsTrue(cancellableTask.IsComplete());

            }
            catch (TaskCanceledException)
            {
                //all g
            }
        }

        private async Task Poll(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Run(() => Thread.Sleep(10), ct);
            }
        }
    }
}
