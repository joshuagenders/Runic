using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Framework.Clients;
using System;
using System.Linq;
using System.Reflection;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestAssemblyManagement
    {
        //bad hack because nunit 3 doesn't work yet and mstest doesnt set cwd to deployment dir
        // and mstestv2 test context has removed the deployment metadata
        private const string wd = "C:\\code\\Runic\\Agent\\src\\Runic.Agent.UnitTest\\bin\\Debug\\netcoreapp1.0";
        private PluginManager _pluginManager { get; set; }

        [TestInitialize]
        public void Init()
        {
            var cli = new[]
            {
                "Agent:MaxThreads=321",
                "Agent:LifetimeSeconds=123",
                "Client:MQConnectionString=MyExampleConnection",
                "Statsd:Port=8125",
                "Statsd:Host=192.168.99.100",
                "Statsd:Prefix=Runic.Stats."
            };
            AgentConfiguration.LoadConfiguration(cli);

            var container = new Startup().Register();
            _pluginManager = container.Resolve<PluginManager>();
            _pluginManager.RegisterProvider(new FilePluginProvider(wd));
        }

        [TestMethod]
        public void TestFunctionTypeRetrieve()
        {
            _pluginManager.LoadPlugin("Runic.ExampleTest");
            var type = _pluginManager.GetFunctionType("Runic.ExampleTest.Functions.FakeFunction");
            Assert.IsNotNull(type);
            Assert.AreEqual(type.Name, "FakeFunction");
        }

        [TestMethod]
        public void TestLoadAssembly()
        {
            _pluginManager.LoadPlugin("Runic.ExampleTest");
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
            _pluginManager.LoadPlugin("Runic.ExampleTest");
            var functions = _pluginManager.GetAvailableFunctions();
            Assert.IsTrue(functions.Any());
        }

        [TestMethod]
        public void TestDualLoadIsSafe()
        {
            _pluginManager.LoadPlugin("Runic.ExampleTest");
            _pluginManager.LoadPlugin("Runic.ExampleTest");
            Assert.AreEqual(_pluginManager.GetAssemblies().Count, 1);
        }

        [TestMethod]
        public void TestGetClassType()
        {
            throw new NotImplementedException();
        }
    }
}
