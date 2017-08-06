using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Clients;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Autofac;

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
                new Mock<IStatsClient>().Object, 
                new LoggerFactory());
        }

        [TestMethod]
        public void AssemblyManagement_FunctionTypeRetrieve()
        {
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            var type = _pluginManager.GetClassType(TestConstants.AssemblyQualifiedClassName);
            Assert.IsNotNull(type);
            Assert.AreEqual(type.Name, "FakeFunction");
        }

        [TestMethod]
        public void AssemblyManagement_LoadAssembly()
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
        public void AssemblyManagement_GetFunctionInfo()
        {
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            var functions = _pluginManager.GetAvailableFunctions();
            Assert.IsTrue(functions.Any());
        }

        [TestMethod]
        public void AssemblyManagement_DualLoadIsSafe()
        {
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            Assert.AreEqual(_pluginManager.GetAssemblies().Count, 1);
        }

        [TestMethod]
        public void AssemblyManagement_AssemblyNotFoundThrowsException()
        {
            Assert.ThrowsException<AssemblyNotFoundException>(() => _pluginManager.LoadPlugin("SomeAssembly"));
        }

        [TestMethod]
        public void AssemblyManagement_ClassNotFoundThrowsException()
        {
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            Assert.ThrowsException<ClassNotFoundInAssemblyException>(() => _pluginManager.GetClassType("SomeClass"));
        }

        [TestMethod]
        public void AssemblyManagement_GetUnloadedPluginThrowsException()
        {
            Assert.ThrowsException<AssemblyNotFoundException>(() => _pluginManager.GetPlugin("someplugin"));
        }

        [TestMethod]
        public void AssemblyManagement_GetAssembliesWithoutLoadReturnsEmptyList()
        {
            Assert.IsFalse(_pluginManager.GetAssemblies().Any());
        }

        [TestMethod]
        public void AssemblyManagement_GetAssemblyKeysWithoutLoadReturnsEmptyList()
        {
            Assert.IsFalse(_pluginManager.GetAssemblyKeys().Any());
        }

        [TestMethod]
        public void AssemblyManagement_GetFunctionsWithoutLoadReturnsEmptyList()
        {
            Assert.IsFalse(_pluginManager.GetAvailableFunctions().Any());
        }

        [TestMethod]
        public void AssemblyManagement_GetClassWithoutLoadThrowsException()
        {
            Assert.ThrowsException<ClassNotFoundInAssemblyException>(() => _pluginManager.GetClassType("SomeClass"));
        }
    }
}
