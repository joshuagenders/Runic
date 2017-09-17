using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.TestHarness.FunctionHarness;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.FunctionHarness
{
    [TestClass]
    public class CancellableTaskTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenCancellableTaskIsCancelled_TaskCompletes()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(2000);
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

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenAsyncCancellableTaskIsCancelled_TaskCompletes()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(2000);
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

        private async Task Poll(CancellationToken ctx = default(CancellationToken))
        {
            while (!ctx.IsCancellationRequested)
            {
                await Task.Run(() => Thread.Sleep(10), ctx);
            }
        }
    }
}
