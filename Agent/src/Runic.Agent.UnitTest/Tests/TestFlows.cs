using System.Collections.Generic;
using Runic.Agent.FlowManagement;
using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.UnitTest.TestUtility;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestFlows
    {
        private AgentWorld _world { get; set; }

        [TestInitialize]
        public void Init()
        {
            _world = new AgentWorld();
        }

        [TestMethod]
        public void TestUpdate()
        {
            var inputFlow = new Flow
            {
                Name = "Test",
                Steps = new List<Step>()
                {
                    new Step() { }
                }
            };
            var updatedInput = new Flow
            {
                Name = "Test",
                Steps = null
            };
            
            _world.FlowManager.AddUpdateFlow(inputFlow);
            _world.FlowManager.AddUpdateFlow(updatedInput);
            var flow = _world.FlowManager.GetFlow("Test");
            Assert.AreEqual("Test", flow.Name);
            Assert.IsNull(flow.Steps);
        }

        [TestMethod]
        public void TestStoreAndGet()
        {
            var inputFlow = new Flow { Name = "Test" };
            _world.FlowManager.AddUpdateFlow(inputFlow);
            var flow = _world.FlowManager.GetFlow("Test");
            Assert.AreEqual(inputFlow, flow);
            Assert.AreEqual("Test", flow.Name);
        }

    }
}
