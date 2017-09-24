using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Agent.Core.Services;

namespace Runic.Agent.Core.UnitTest.Tests.FlowManagement
{
    [TestClass]
    public class FlowInitialiserTests
    {
        private Mock<IPluginManager> _pluginManager { get; set; }

        [TestInitialize]
        public void Init()
        {
            _pluginManager = new Mock<IPluginManager>();
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenInitialiseFlow_PluginsAreLoaded()
        {
            var flow = TestData.GetTestFlowSingleStep;   
            var flowInitialiser = new JourneyInitialiser(_pluginManager.Object, new Mock<IEventService>().Object);
            flowInitialiser.InitialiseFlow(flow);
            _pluginManager.Verify(p => p.LoadPlugin(TestConstants.AssemblyName));
        }
    }
}
