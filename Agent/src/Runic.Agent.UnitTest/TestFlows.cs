using System.Collections.Generic;
using Runic.Agent.FlowManagement;
using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runic.Agent.UnitTest
{
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

            var flows = new Flows();
            flows.AddUpdateFlow(inputFlow);
            flows.AddUpdateFlow(updatedInput);
            var flow = flows.GetFlow("Test");
            Assert.AreEqual("Test", flow.Name);
            Assert.IsNull(flow.Steps);
        }

        [TestMethod]
        public void TestStoreAndGet()
        {
            var inputFlow = new Flow { Name = "Test" };
            var flows = new Flows();
            flows.AddUpdateFlow(inputFlow);
            var flow = flows.GetFlow("Test");
            Assert.AreEqual(inputFlow, flow);
            Assert.AreEqual("Test", flow.Name);
        }

    }
}
