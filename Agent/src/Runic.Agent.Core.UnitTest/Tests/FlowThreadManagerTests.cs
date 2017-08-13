using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Models;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class FlowThreadManagerTests
    {
        private TestEnvironment _testEnvironment { get; set; }
        private Flow _flow { get; set; }
        private FlowThreadManager _manager { get; set; }

        [TestInitialize]
        public void Init()
        {
            _testEnvironment = new TestEnvironment();
            _flow = TestData.GetTestFlowSingleStep;
            _manager = new FlowThreadManager(
                _flow,
                _testEnvironment.Stats.Object,
                _testEnvironment.RunnerService.Object,
                new Mock<ILoggingHandler>().Object);
        }

        [TestMethod]
        public async Task WhenUpdatingThreadCount_ThreadCountIsUpdated()
        {
            await _manager.UpdateThreadCountAsync(1);
            Assert.AreEqual(1, _manager.GetCurrentThreadCount());
            await _manager.UpdateThreadCountAsync(0);
            Assert.AreEqual(0, _manager.GetCurrentThreadCount());
            //todo verify mocks
        }

        [TestMethod]
        public async Task WhenStopAll_ThreadCountIsZero()
        {
            await _manager.UpdateThreadCountAsync(1);
            Assert.AreEqual(1, _manager.GetCurrentThreadCount());
            _manager.StopAll();
            Assert.AreEqual(0, _manager.GetCurrentThreadCount());
            //todo verify mocks
        }
    }
}
