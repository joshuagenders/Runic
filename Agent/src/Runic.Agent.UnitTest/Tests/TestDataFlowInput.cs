using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.UnitTest.TestUtility;
using System;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestDataFlowInput
    {
        private TestEnvironment _testEnvironment { get; set; }
        
        [TestInitialize]
        public void Initialise()
        {
            _testEnvironment = new TestEnvironment();
        }

        [TestMethod]
        public void TestSingleMessageInput()
        {
            throw new NotImplementedException();
            //start flow

            //insert mock message

            //validate message has been used
        }
    }
}
