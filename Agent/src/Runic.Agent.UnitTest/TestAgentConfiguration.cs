using NUnit.Framework;
using Runic.Agent.Configuration;

namespace Runic.Agent.UnitTest
{
    public class TestAgentConfiguration
    {
        [Test]
        public void TestCommandLineInputs()
        {
            var cli = new[]
            {
                "Agent:MaxThreads=321",
                "Agent:LifetimeSeconds=123",
                "Client:MQConnectionString=MyExampleConnection",
                "Statsd:Port=8000",
                "Statsd:Host=TestHost",
                "Statsd:Prefix=MyPrefix"
            };
            AgentConfiguration.LoadConfiguration(cli);

            Assert.AreEqual(AgentConfiguration.ClientConnectionConfiguration, "MyExampleConnection");
            Assert.AreEqual(AgentConfiguration.LifetimeSeconds, 123);
            Assert.AreEqual(AgentConfiguration.MaxThreads, 321);
            Assert.AreEqual(AgentConfiguration.StatsdHost, "TestHost");
            Assert.AreEqual(AgentConfiguration.StatsdPort, 8000);
            Assert.AreEqual(AgentConfiguration.StatsdPrefix, "MyPrefix");
        }
    }
}
