using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.UnitTest.TestUtility;
using System.Linq;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestFlowInitialiser
    {
        private TestEnvironment _testEnvironment { get; set; }

        [TestInitialize]
        public void Init()
        {
            _testEnvironment = new TestEnvironment();
        }

        [TestMethod]
        public void TestLibraryLoads()
        {
            var flow = TestData.GetTestFlowSingleStep;   
            var flowInitialiser = new FlowInitialiser(_testEnvironment.App.PluginManager);
            flowInitialiser.InitialiseFlow(flow);

            Assert.IsTrue(_testEnvironment.App.PluginManager.GetAssemblies().Any());
            Assert.IsTrue(_testEnvironment.App.PluginManager.GetAssemblyKeys().Any(k => k == TestConstants.AssemblyName));
        }
    }
}
