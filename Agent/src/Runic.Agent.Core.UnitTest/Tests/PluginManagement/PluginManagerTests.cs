﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Clients;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Runic.Agent.Core.Exceptions;
using Runic.Agent.Core.ExternalInterfaces;

namespace Runic.Agent.Core.UnitTest.Tests.PluginManagement
{
    [TestClass]
    public class PluginManagerTests
    {
        private PluginManager _pluginManager { get; set; }

        [TestInitialize]
        public void Init()
        {
            _pluginManager = new PluginManager(
                new Mock<IRuneClient>().Object, 
                new FilePluginProvider(Directory.GetCurrentDirectory()),
                new Mock<IStatsClient>().Object, 
                new Mock<ILoggingHandler>().Object);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenAssemblyIsLoaded_FunctionTypeisRetrieved()
        {
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            var type = _pluginManager.GetClassType(TestConstants.AssemblyQualifiedClassName);
            Assert.IsNotNull(type);
            Assert.AreEqual(type.Name, "FakeFunction");
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenAssemblyIsLoaded_RuneClientPropertyCanBeLocated()
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

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingFunctionInfo_ReturnsFunctionInfo()
        {
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            var functions = _pluginManager.GetAvailableFunctions();
            Assert.IsTrue(functions.Any());
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenAssemblyIsLoadedTwice_AssemblyIsLoadedOnce()
        {
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            Assert.AreEqual(_pluginManager.GetAssemblies().Count, 1);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenLoadingMissingPlugin_ThrowsException()
        {
            Assert.ThrowsException<AssemblyNotFoundException>(() => _pluginManager.LoadPlugin("SomeAssembly"));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenLoadingAMissingClass_ThrowsException()
        {
            _pluginManager.LoadPlugin(TestConstants.AssemblyName);
            Assert.ThrowsException<ClassNotFoundInAssemblyException>(() => _pluginManager.GetClassType("SomeClass"));
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

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingClassWithoutLoad_ThrowsException()
        {
            Assert.ThrowsException<ClassNotFoundInAssemblyException>(() => _pluginManager.GetClassType("SomeClass"));
        }
    }
}