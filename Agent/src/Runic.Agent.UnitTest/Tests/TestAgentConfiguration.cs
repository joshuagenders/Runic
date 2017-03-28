using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Configuration;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestAgentConfiguration
    {
        [TestMethod]
        public void TestCommandLineInputs()
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

            Assert.AreEqual(AgentConfiguration.Instance.ClientConnectionConfiguration, "MyExampleConnection");
            Assert.AreEqual(AgentConfiguration.Instance.LifetimeSeconds, 123);
            Assert.AreEqual(AgentConfiguration.Instance.MaxThreads, 321);
            Assert.AreEqual(AgentConfiguration.Instance.StatsdHost, "192.168.99.100");
            Assert.AreEqual(AgentConfiguration.Instance.StatsdPort, 8125);
            Assert.AreEqual(AgentConfiguration.Instance.StatsdPrefix, "Runic.Stats.");
        }
    }
}
