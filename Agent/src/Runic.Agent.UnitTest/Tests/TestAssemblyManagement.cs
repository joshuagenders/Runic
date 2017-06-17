using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.UnitTest.TestUtility;
using Runic.Framework.Clients;
using System.Linq;
using System.Reflection;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestAssemblyManagement
    {
        private TestEnvironment _testEnvironment { get; set; }

        [TestInitialize]
        public void Init()
        {
            _testEnvironment = new TestEnvironment();
        }

        [TestMethod]
        public void TestFunctionTypeRetrieve()
        {
            _testEnvironment.App.PluginManager.LoadPlugin("Runic.ExampleTest");
            var type = _testEnvironment.App.PluginManager.GetClassType("Runic.ExampleTest.Functions.FakeFunction");
            Assert.IsNotNull(type);
            Assert.AreEqual(type.Name, "FakeFunction");
        }

        [TestMethod]
        public void TestLoadAssembly()
        {
            _testEnvironment.App.PluginManager.LoadPlugin("Runic.ExampleTest");
            Assert.AreEqual(_testEnvironment.App.PluginManager.GetAssemblies().Count, 1);
            var assembly = _testEnvironment.App.PluginManager.GetAssemblies().Single();
            var iocType = assembly.GetType("Runic.ExampleTest.RunicIoC");
            var runeClient = iocType.GetProperties(BindingFlags.Static | BindingFlags.Public)
                                    .Where(t => t.PropertyType.IsAssignableTo<IRuneClient>())
                                    .Select(t => t.GetValue(iocType));
            Assert.IsNotNull(runeClient);
        }

        [TestMethod]
        public void TestGetFunctionInfo()
        {
            _testEnvironment.App.PluginManager.LoadPlugin("Runic.ExampleTest");
            var functions = _testEnvironment.App.PluginManager.GetAvailableFunctions();
            Assert.IsTrue(functions.Any());
        }

        [TestMethod]
        public void TestDualLoadIsSafe()
        {
            _testEnvironment.App.PluginManager.LoadPlugin("Runic.ExampleTest");
            _testEnvironment.App.PluginManager.LoadPlugin("Runic.ExampleTest");
            Assert.AreEqual(_testEnvironment.App.PluginManager.GetAssemblies().Count, 1);
        }
    }
}
