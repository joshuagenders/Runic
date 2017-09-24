using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.StepController
{
    [TestClass]
    public class FlowThreadManagerTests
    {
        private Journey _flow { get; set; }
        private JourneyControl _manager { get; set; }
        private Mock<IPerson> _runnerService { get; set; }
        [TestInitialize]
        public void Init()
        {
            _flow = TestData.GetTestFlowSingleStep;
            _runnerService = new Mock<IPerson>();
            _manager = new JourneyControl(_flow, _runnerService.Object, new Mock<IEventService>().Object);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenUpdatingThreadCount_ThreadCountIsUpdated()
        {
            await _manager.UpdateThreadCountAsync(1);
            Assert.AreEqual(1, _manager.GetCurrentThreadCount());
            await _manager.UpdateThreadCountAsync(0);
            Assert.AreEqual(0, _manager.GetCurrentThreadCount());
            _runnerService.Verify(r => r.PerformJourneyAsync(_flow, It.IsAny<CancellationToken>()));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenStopAll_ThreadCountIsZero()
        {
            await _manager.UpdateThreadCountAsync(1);
            Assert.AreEqual(1, _manager.GetCurrentThreadCount());
            _manager.StopAll();
            Assert.AreEqual(0, _manager.GetCurrentThreadCount());
            _runnerService.Verify(r => r.PerformJourneyAsync(_flow, It.IsAny<CancellationToken>()));
        }
    }
}
