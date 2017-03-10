using System.Collections.Generic;
using NUnit.Framework;
using Runic.Agent.FlowManagement;
using Runic.Core.Models;

namespace Runic.Agent.UnitTest
{
    public class TestFlows
    {
        [Test]
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

        [Test]
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
