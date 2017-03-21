using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Framework.Clients;
using System.Linq;
using System.Reflection;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestAssemblyManagement
    {
        private AgentWorld _world { get; set; }

        [TestInitialize]
        public void Init()
        {
            _world = new AgentWorld();
        }

        [TestMethod]
        public void TestFunctionTypeRetrieve()
        {
            _world.PluginManager.LoadPlugin("Runic.ExampleTest");
            var type = _world.PluginManager.GetClassType("Runic.ExampleTest.Functions.FakeFunction");
            Assert.IsNotNull(type);
            Assert.AreEqual(type.Name, "FakeFunction");
        }

        [TestMethod]
        public void TestLoadAssembly()
        {
            _world.PluginManager.LoadPlugin("Runic.ExampleTest");
            Assert.AreEqual(_world.PluginManager.GetAssemblies().Count, 1);
            var assembly = _world.PluginManager.GetAssemblies().Single();
            var iocType = assembly.GetType("Runic.ExampleTest.RunicIoC");
            var runeClient = iocType.GetProperties(BindingFlags.Static | BindingFlags.Public)
                                    .Where(t => t.PropertyType.IsAssignableTo<IRuneClient>())
                                    .Select(t => t.GetValue(iocType));
            Assert.IsNotNull(runeClient);
        }

        [TestMethod]
        public void TestGetFunctionInfo()
        {
            _world.PluginManager.LoadPlugin("Runic.ExampleTest");
            var functions = _world.PluginManager.GetAvailableFunctions();
            Assert.IsTrue(functions.Any());
        }

        [TestMethod]
        public void TestDualLoadIsSafe()
        {
            _world.PluginManager.LoadPlugin("Runic.ExampleTest");
            _world.PluginManager.LoadPlugin("Runic.ExampleTest");
            Assert.AreEqual(_world.PluginManager.GetAssemblies().Count, 1);
        }
    }
}
