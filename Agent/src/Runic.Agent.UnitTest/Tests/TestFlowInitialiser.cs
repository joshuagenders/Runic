using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Harness;
using Runic.Agent.UnitTest.TestUtility;
using Runic.Framework.Models;
using System.Collections.Generic;
using System.Linq;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestFlowInitialiser
    {
        private AgentWorld _world { get; set; }

        [TestInitialize]
        public void Init()
        {
            _world = new AgentWorld();
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

            
            var flowInitialiser = new FlowInitialiser(_world.PluginManager);
            flowInitialiser.InitialiseFlow(flow);

            Assert.IsTrue(_world.PluginManager.GetAssemblies().Any());
            Assert.IsTrue(_world.PluginManager.GetAssemblyKeys().Any(k => k == "Runic.ExampleTest"));
        }
    }
}
