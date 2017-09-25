﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.TestHarness.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.Services
{
    [TestClass]
    public class DatetimeServiceTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenWaitingUntil_DatetimeServiceWaits()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var service = new DateTimeService();
            var expectedFinish = DateTime.Now.AddSeconds(1);
            await service.WaitMilliseconds(1000, cts.Token);
            Assert.IsTrue(DateTime.Now >= expectedFinish);
        }
    }
}