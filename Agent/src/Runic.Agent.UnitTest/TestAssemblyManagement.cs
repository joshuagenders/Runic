﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using System;
using System.IO;
using System.Linq;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestAssemblyManagement
    {
        //bad hack because nunit 3 doesn't work yet and mstest doesnt set cwd to deployment dir
        // and mstestv2 test context has removed the deployment metadata
        private const string wd = "C:\\code\\Runic\\Agent\\src\\Runic.Agent.UnitTest\\bin\\Debug\\netcoreapp1.0";

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

            var container = new Startup().RegisterDependencies();
        }

        [TestCleanup]
        public void Cleanup()
        {
            PluginManager.ClearAssemblies();
        }

        [TestMethod]
        public void TestFunctionTypeRetrieve()
        {
            PluginManager.LoadPlugin("Runic.ExampleTest", new FilePluginProvider(wd));
            var type = PluginManager.GetFunctionType("Runic.ExampleTest.Functions.FakeFunction");
            Assert.IsNotNull(type);
            Assert.AreEqual(type.Name, "FakeFunction");
        }

        [TestMethod]
        public void TestLoadAssembly()
        {
            PluginManager.LoadPlugin("Runic.ExampleTest", new FilePluginProvider(wd));
            Assert.AreEqual(PluginManager.GetAssemblies().Count, 1);
        }

        [TestMethod]
        public void TestGetFunctionInfo()
        {
            PluginManager.LoadPlugin("Runic.ExampleTest", new FilePluginProvider(wd));
            var functions = PluginManager.GetAvailableFunctions();
            Assert.IsTrue(functions.Any());
        }

        [TestMethod]
        public void TestDualLoadIsSafe()
        {
            PluginManager.LoadPlugin("Runic.ExampleTest", new FilePluginProvider(wd));
            PluginManager.LoadPlugin("Runic.ExampleTest", new FilePluginProvider(wd));
            Assert.AreEqual(PluginManager.GetAssemblies().Count, 1);
        }
    }
}
