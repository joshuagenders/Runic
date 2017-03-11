using System.Collections.Generic;
using Runic.Agent.FlowManagement;
using Runic.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runic.Agent.UnitTest {

    [TestClass]
    public class TestFlows
    {
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

            Flows.AddUpdateFlow(inputFlow);
            Flows.AddUpdateFlow(updatedInput);
            var flow = Flows.GetFlow("Test");
            Assert.AreEqual("Test", flow.Name);
            Assert.IsNull(flow.Steps);
        }

        [TestMethod]
        public void TestStoreAndGet()
        {
            var inputFlow = new Flow { Name = "Test" };
            Flows.AddUpdateFlow(inputFlow);
            var flow = Flows.GetFlow("Test");
            Assert.AreEqual(inputFlow, flow);
            Assert.AreEqual("Test", flow.Name);
        }

    }
}
