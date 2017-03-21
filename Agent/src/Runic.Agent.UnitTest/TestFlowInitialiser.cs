using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.Harness;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestFlowInitialiser
    {
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

            _pluginManager = new PluginManager();
            _pluginManager.RegisterProvider(container.Resolve<IPluginProvider>());
        }
        [TestMethod]
        public void TestLibraryLoads()
        {

            var flow = new Flow()
            {
                Name = "Test",
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "blah",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.ExampleTest",
                            FunctionName = "FakeFunction",
                            AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.FakeFunction"
                        }
                    }
                }
            };

            
            var flowInitialiser = new FlowInitialiser(_pluginManager);
            flowInitialiser.InitialiseFlow(flow);

            Assert.IsTrue(_pluginManager.GetAssemblies().Any());
            //Assert.IsTrue(_pluginManager.GetAssemblyKeys().Any(k => k == "Runic.ExampleTest"));

        }
    }
}
