using System.IO;
using Runic.Agent.AssemblyManagement;
using NUnit.Framework;

namespace Runic.Agent.UnitTest
{
    public class TestAssemblyManagement
    {
        [Test]
        public void TestFunctionTypeRetrieve()
        {
            PluginManager.LoadPlugin("ExampleTest", new FilePluginProvider(null));
            var type = PluginManager.GetFunctionType("Runic.ExampleTest.FakeAction");
            Assert.IsNotNull(type);
            Assert.AreEqual(type.Name, "FakeFunction");
        }

        [Test]
        public void TestLoadAssembly()
        {
            PluginManager.LoadPlugin("ExampleTest", new FilePluginProvider(Directory.GetCurrentDirectory()));
            Assert.AreEqual(PluginManager.GetAssemblies().Count, 1);
        }

        [Test]
        public void TestDualLoadIsSafe()
        {
            PluginManager.LoadPlugin("ExampleTest", new FilePluginProvider(Directory.GetCurrentDirectory()));
            PluginManager.LoadPlugin("ExampleTest", new FilePluginProvider(Directory.GetCurrentDirectory()));
            Assert.AreEqual(PluginManager.GetAssemblies().Count, 1);
        }
    }
}
