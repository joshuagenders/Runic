using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Clients;
using System.IO;
using System.Linq;
using System.Reflection;
using Runic.Agent.Core.Metrics;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestAssemblyManagement
    {
        private PluginManager _pluginManager { get; set; }

        [TestInitialize]
        public void Init()
        {
            _pluginManager = new PluginManager(
                new Mock<IRuneClient>().Object, 
                new FilePluginProvider(Directory.GetCurrentDirectory()),
                new Mock<IStats>().Object);
        }

        [TestMethod]
        public void TestFunctionTypeRetrieve()
        {
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            var type = _pluginManager.GetClassType(TestConstants.AssemblyQualifiedClassName);
            Assert.IsNotNull(type);
            Assert.AreEqual(type.Name, "FakeFunction");
        }

        [TestMethod]
        public void TestLoadAssembly()
        {
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            Assert.AreEqual(_pluginManager.GetAssemblies().Count, 1);
            var assembly = _pluginManager.GetAssemblies().Single();
            var iocType = assembly.GetType("Runic.ExampleTest.RunicIoC");
            var runeClient = iocType.GetProperties(BindingFlags.Static | BindingFlags.Public)
                                    .Where(t => t.PropertyType.IsAssignableTo<IRuneClient>())
                                    .Select(t => t.GetValue(iocType));
            Assert.IsNotNull(runeClient);
        }

        [TestMethod]
        public void TestGetFunctionInfo()
        {
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            var functions = _pluginManager.GetAvailableFunctions();
            Assert.IsTrue(functions.Any());
        }

        [TestMethod]
        public void TestDualLoadIsSafe()
        {
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            Assert.AreEqual(_pluginManager.GetAssemblies().Count, 1);
        }
    }
}
