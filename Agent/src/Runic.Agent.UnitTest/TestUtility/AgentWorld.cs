using Moq;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.Messaging;

namespace Runic.Agent.UnitTest.TestUtility
{
    public class AgentWorld
    {
        private const string wd = "C:\\code\\Runic\\Agent\\src\\Runic.Agent.UnitTest\\bin\\Debug\\netcoreapp1.0";
        public PluginManager PluginManager { get; set; }
        public IMessagingService MessagingService { get; set; }

        public AgentWorld()
        {
            //init agent
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
            var container = new Startup().Register();

            PluginManager = new PluginManager();
            PluginManager.RegisterProvider(new FilePluginProvider(wd));

            MessagingService = new Mock<IMessagingService>().Object;
        }
    }
}
