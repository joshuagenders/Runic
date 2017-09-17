﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Agent.Framework.Clients;
using System.Linq;
using Runic.Agent.Core.Services;
using Runic.Agent.Framework.ExternalInterfaces;
using System.Reflection;

namespace Runic.Agent.Core.UnitTest.Tests.PluginManagement
{
    [TestClass]
    public class PluginManagerTests
    {
        private PluginManager _pluginManager { get; set; }
        private Mock<IPluginProvider> _pluginProvider { get; set; }

        [TestInitialize]
        public void Init()
        {
            _pluginProvider = new Mock<IPluginProvider>();
            _pluginManager = new PluginManager(
                new Mock<IRuneClient>().Object, 
                _pluginProvider.Object,
                new Mock<IEventService>().Object);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenLoadingMissingPlugin_ThrowsException()
        {
            Assert.ThrowsException<AssemblyNotFoundException>(() => _pluginManager.LoadPlugin("SomeAssembly"));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingUnloadedPlugin_ThrowsException()
        {
            Assert.ThrowsException<AssemblyNotFoundException>(() => _pluginManager.GetPlugin("someplugin"));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingAssembliesWithoutLoad_ReturnsEmptyList()
        {
            Assert.IsFalse(_pluginManager.GetAssemblies().Any());
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingAssemblyKeysWithoutLoad_ReturnsEmptyList()
        {
            Assert.IsFalse(_pluginManager.GetAssemblyKeys().Any());
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingFunctionsWithoutLoad_ReturnsEmptyList()
        {
            Assert.IsFalse(_pluginManager.GetAvailableFunctions().Any());
        }
    }
}
