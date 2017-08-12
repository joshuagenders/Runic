using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestDatetimeService
    {
        [TestMethod]
        public async Task WhenWaitingUntil_DatetimeServiceWaits()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var service = new DateTimeService();
            var expectedFinish = DateTime.Now.AddSeconds(1);
            await service.WaitUntil(1000, cts.Token);
            Assert.IsTrue(DateTime.Now >= expectedFinish);
        }
    }
}
