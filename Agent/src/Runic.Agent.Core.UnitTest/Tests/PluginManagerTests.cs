using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Configuration;
using System.Linq;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class PluginManagerTests
    {
        private AssemblyManager _pluginManager { get; set; }

        [TestInitialize]
        public void Init()
        {
            var config = new Mock<ICoreConfiguration>();
            config.Setup(c => c.PluginFolderPath).Returns("/plugins/");
            config.Setup(c => c.TaskCreationPollingIntervalSeconds).Returns(2);
            _pluginManager = new AssemblyManager(config.Object);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenLoadingMissingPlugin_ThrowsException()
        {
            Assert.ThrowsException<AssemblyLoadException>(() => _pluginManager.LoadAssembly("SomeAssembly"));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingUnloadedPlugin_ThrowsException()
        {
            Assert.ThrowsException<AssemblyLoadException>(() => _pluginManager.GetAssembly("someplugin"));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingAssembliesWithoutLoad_ReturnsEmptyList()
        {
            Assert.IsFalse(_pluginManager.GetAssemblies().Any());
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingAssemblyKeysWithoutLoad_ReturnsEmptyList()
        {
            Assert.IsFalse(_pluginManager.GetAssemblyKeys().Any());
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingMethodsWithoutLoad_ReturnsEmptyList()
        {
            Assert.IsFalse(_pluginManager.GetAvailableMethods().Any());
        }
    }
}
