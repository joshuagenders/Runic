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
                "Statsd:Port=8125",
                "Statsd:Host=localhost",
                "Statsd:Prefix=Runic.Stats."
            };
            AgentConfiguration.LoadConfiguration(cli);

            Assert.AreEqual(AgentConfiguration.ClientConnectionConfiguration, "MyExampleConnection");
            Assert.AreEqual(AgentConfiguration.LifetimeSeconds, 123);
            Assert.AreEqual(AgentConfiguration.MaxThreads, 321);
            Assert.AreEqual(AgentConfiguration.StatsdHost, "localhost");
            Assert.AreEqual(AgentConfiguration.StatsdPort, 8125);
            Assert.AreEqual(AgentConfiguration.StatsdPrefix, "Runic.Stats.");
        }
    }
}
