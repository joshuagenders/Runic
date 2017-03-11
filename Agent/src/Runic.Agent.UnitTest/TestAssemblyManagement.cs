using System.IO;
using Runic.Agent.AssemblyManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestAssemblyManagement
    {

        [TestInitialize]
        public void Init()
        {
            IoC.RegisterDependencies(new Startup());
        }

        [TestMethod]
        public void TestFunctionTypeRetrieve()
        {
            PluginManager.LoadPlugin("ExampleTest", new FilePluginProvider(null));
            var type = PluginManager.GetFunctionType("Runic.ExampleTest.Functions.FakeFunction");
            Assert.IsNotNull(type);
            Assert.AreEqual(type.Name, "FakeFunction");
        }

        [TestMethod]
        public void TestLoadAssembly()
        {
            PluginManager.LoadPlugin("ExampleTest", new FilePluginProvider(Directory.GetCurrentDirectory()));
            Assert.AreEqual(PluginManager.GetAssemblies().Count, 1);
        }

        [TestMethod]
        public void TestDualLoadIsSafe()
        {
            PluginManager.LoadPlugin("ExampleTest", new FilePluginProvider(Directory.GetCurrentDirectory()));
            PluginManager.LoadPlugin("ExampleTest", new FilePluginProvider(Directory.GetCurrentDirectory()));
            Assert.AreEqual(PluginManager.GetAssemblies().Count, 1);
        }
    }
}
