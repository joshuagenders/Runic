using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Agent.Framework.Clients;
using System.IO;
using System.Linq;
using Runic.Agent.Core.Services;

namespace Runic.Agent.Core.UnitTest.Tests.FlowManagement
{
    [TestClass]
    public class FlowInitialiserTests
    {
        private PluginManager _pluginManager { get; set; }
        [TestInitialize]
        public void Init()
        {
            _pluginManager = new PluginManager(
                new Mock<IRuneClient>().Object, 
                new FilePluginProvider(Directory.GetCurrentDirectory()),
                new Mock<IEventService>().Object);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenInitialiseFlow_PluginsAreLoaded()
        {
            var flow = TestData.GetTestFlowSingleStep;   
            var flowInitialiser = new FlowInitialiser(_pluginManager, new Mock<IEventService>().Object);
            flowInitialiser.InitialiseFlow(flow);

            Assert.IsTrue(_pluginManager.GetAssemblies().Any());
            Assert.IsTrue(_pluginManager.GetAssemblyKeys().Any(k => k == TestConstants.AssemblyName));
        }
    }
}
