using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Models;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestFlowThreadManager
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
                new FunctionFactory(
                    _testEnvironment.PluginManager.Object,
                    _testEnvironment.Stats.Object,
                    _testEnvironment.DataService.Object,
                    new LoggerFactory()),
                    null,
                    new LoggerFactory());
        }

        [TestMethod]
        public async Task FlowThreadManager_UpdateThreadSetsThreadCount()
        {
            await _manager.UpdateThreadCountAsync(1);
            Assert.AreEqual(1, _manager.GetCurrentThreadCount());
            await _manager.UpdateThreadCountAsync(0);
            Assert.AreEqual(0, _manager.GetCurrentThreadCount());
        }

        [TestMethod]
        public async Task FlowThreadManager_StopAllStopsAll()
        {
            await _manager.UpdateThreadCountAsync(1);
            Assert.AreEqual(1, _manager.GetCurrentThreadCount());
            _manager.StopAll();
            Assert.AreEqual(0, _manager.GetCurrentThreadCount());
        }
    }
}
