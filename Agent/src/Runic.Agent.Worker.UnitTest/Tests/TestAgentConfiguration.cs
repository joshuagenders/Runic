using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Worker.Configuration;

namespace Runic.Agent.Worker.UnitTest.Tests
{
    [TestClass]
    public class TestAgentConfiguration
    {
        [TestMethod]
        public void TestCommandLineInputs()
        {
            
            WorkerConfiguration.LoadConfiguration(TestConstants.CommandLineArguments);

            Assert.AreEqual(WorkerConfiguration.Instance.ClientConnectionConfiguration, "MyExampleConnection");
            Assert.AreEqual(WorkerConfiguration.Instance.LifetimeSeconds, 123);
            Assert.AreEqual(WorkerConfiguration.Instance.MaxThreads, 321);
            Assert.AreEqual(WorkerConfiguration.Instance.StatsdHost, "192.168.99.100");
            Assert.AreEqual(WorkerConfiguration.Instance.StatsdPort, 8125);
            Assert.AreEqual(WorkerConfiguration.Instance.StatsdPrefix, "Runic.Stats.");
        }
    }
}
