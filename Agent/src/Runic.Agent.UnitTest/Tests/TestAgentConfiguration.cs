using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Configuration;
using Runic.Agent.UnitTest.TestUtility;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestAgentConfiguration
    {
        [TestMethod]
        public void TestCommandLineInputs()
        {
            
            AgentConfiguration.LoadConfiguration(TestConstants.CommandLineArguments);

            Assert.AreEqual(AgentConfiguration.Instance.ClientConnectionConfiguration, "MyExampleConnection");
            Assert.AreEqual(AgentConfiguration.Instance.LifetimeSeconds, 123);
            Assert.AreEqual(AgentConfiguration.Instance.MaxThreads, 321);
            Assert.AreEqual(AgentConfiguration.Instance.StatsdHost, "192.168.99.100");
            Assert.AreEqual(AgentConfiguration.Instance.StatsdPort, 8125);
            Assert.AreEqual(AgentConfiguration.Instance.StatsdPrefix, "Runic.Stats.");
        }
    }
}
