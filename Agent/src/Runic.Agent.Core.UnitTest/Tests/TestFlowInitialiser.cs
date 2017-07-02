﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Metrics;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Clients;
using System.IO;
using System.Linq;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestFlowInitialiser
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
        public void FlowInitialiser_TestLibraryLoads()
        {
            var flow = TestData.GetTestFlowSingleStep;   
            var flowInitialiser = new FlowInitialiser(_pluginManager);
            flowInitialiser.InitialiseFlow(flow);

            Assert.IsTrue(_pluginManager.GetAssemblies().Any());
            Assert.IsTrue(_pluginManager.GetAssemblyKeys().Any(k => k == TestConstants.AssemblyName));
        }
    }
}
