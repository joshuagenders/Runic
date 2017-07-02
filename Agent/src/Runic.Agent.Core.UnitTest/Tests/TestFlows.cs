using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Agent.Core.FlowManagement;
using Moq;
using Runic.Agent.Core.Metrics;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestFlows
    {
        private IFlowManager _flowManager { get; set; }

        [TestInitialize]
        public void Init()
        {
            _flowManager = new FlowManager(new Mock<IStats>().Object);
        }

        [TestMethod]
        public void FlowManager_TestUpdate()
        {
            var inputFlow = TestData.GetTestFlowSingleStep;
            var updatedInput = TestData.GetTestFlowTwoSteps;

            _flowManager.AddUpdateFlow(inputFlow);
            _flowManager.AddUpdateFlow(updatedInput);
            var flow = _flowManager.GetFlow("Test");
            Assert.AreEqual("Test", flow.Name);
            Assert.AreEqual(2, flow.Steps.Count);
        }

        [TestMethod]
        public void FlowManager_TestStoreAndGet()
        {
            var inputFlow = new Flow { Name = "Test" };
            _flowManager.AddUpdateFlow(inputFlow);
            var flow = _flowManager.GetFlow("Test");
            Assert.AreEqual(inputFlow, flow);
            Assert.AreEqual("Test", flow.Name);
        }
    }
}
