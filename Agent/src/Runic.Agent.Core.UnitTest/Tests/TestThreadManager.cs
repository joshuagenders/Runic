using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestThreadManager
    {
        private TestEnvironment _testEnvironment { get; set; }

        [TestInitialize]
        public void Init()
        {
            _testEnvironment = new TestEnvironment();
        }

        [TestMethod]
        public async Task ThreadManager_TestUpdateThreads()
        {
            var flow = TestData.GetTestFlowSingleStep;
        
            var manager = new FlowThreadManager(
                flow,
                _testEnvironment.Stats.Object, 
                new FunctionFactory(
                    flow, 
                    _testEnvironment.PluginManager, 
                    _testEnvironment.Stats.Object, 
                    _testEnvironment.DataService.Object));
        
            await manager.SafeUpdateThreadCountAsync(1);
            Assert.AreEqual(1, manager.GetCurrentThreadCount());
            await manager.SafeUpdateThreadCountAsync(0);
            Assert.AreEqual(0, manager.GetCurrentThreadCount());
        }
    }
}
