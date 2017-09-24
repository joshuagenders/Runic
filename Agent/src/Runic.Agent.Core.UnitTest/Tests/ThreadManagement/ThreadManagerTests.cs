using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.ThreadManagement
{
    [TestClass]
    public class ThreadManagerTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenSettingThreadLevel_ThenLevelIsSet()
        {
            var mockRunnerService = new Mock<IPerson>();
            var mockEventService = new Mock<IEventService>();
            var threadManager = new PopulationControl(mockRunnerService.Object, mockEventService.Object);
            var flow = new Framework.Models.Journey();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            await threadManager.SetThreadLevelAsync("flowid", flow, 1, cts.Token);
            mockEventService.Verify(e => e.OnThreadChange(flow, 1));
            mockRunnerService.Verify(r => r.PerformJourneyAsync(flow, It.IsAny<CancellationToken>()));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenCancellingAll_ThenThreadCountIsZero()
        {
            var mockRunnerService = new Mock<IPerson>();
            var mockEventService = new Mock<IEventService>();
            var threadManager = new PopulationControl(mockRunnerService.Object, mockEventService.Object);
            var flow = new Framework.Models.Journey();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            await threadManager.SetThreadLevelAsync("flowid", flow, 1, cts.Token);
            await threadManager.CancelAll();

            mockEventService.Verify(e => e.OnThreadChange(flow, 1));
            mockEventService.Verify(e => e.OnThreadChange(flow, 0));
            mockRunnerService.Verify(r => r.PerformJourneyAsync(flow, It.IsAny<CancellationToken>()));
        }
    }
}
