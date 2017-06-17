using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.UnitTest.TestUtility;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestFlows
    {
        private TestEnvironment _testEnvironment { get; set; }

        [TestInitialize]
        public void Init()
        {
            _testEnvironment = new TestEnvironment();
        }

        [TestMethod]
        public void TestUpdate()
        {
            var inputFlow = TestData.GetTestFlowSingleStep;
            var updatedInput = TestData.GetTestFlowTwoSteps;

            _testEnvironment.App.FlowManager.AddUpdateFlow(inputFlow);
            _testEnvironment.App.FlowManager.AddUpdateFlow(updatedInput);
            var flow = _testEnvironment.App.FlowManager.GetFlow("Test");
            Assert.AreEqual("Test", flow.Name);
            Assert.AreEqual(2, flow.Steps.Count);
        }

        [TestMethod]
        public void TestStoreAndGet()
        {
            var inputFlow = new Flow { Name = "Test" };
            _testEnvironment.App.FlowManager.AddUpdateFlow(inputFlow);
            var flow = _testEnvironment.App.FlowManager.GetFlow("Test");
            Assert.AreEqual(inputFlow, flow);
            Assert.AreEqual("Test", flow.Name);
        }
    }
}
