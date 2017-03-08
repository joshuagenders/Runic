using System.IO;
using Runic.Agent.AssemblyManagement;
using NUnit.Framework;
using Runic.Core.Attributes;

namespace Runic.Agent.UnitTest
{
    public class TestAssemblyManagement
    {

        [ClassInitialise]
        public void Init()
        {
            IoC.RegisterDependencies(new Startup());
        }

        [Test]
        public void TestFunctionTypeRetrieve()
        {
            PluginManager.LoadPlugin("ExampleTest", new FilePluginProvider(null));
            var type = PluginManager.GetFunctionType("Runic.ExampleTest.Functions.FakeFunction");
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
