using Autofac;
using Moq;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.Data;
using Runic.Agent.FlowManagement;
using Runic.Agent.Messaging;
using Runic.Agent.Metrics;
using Runic.Agent.Service;

namespace Runic.Agent.UnitTest.TestUtility
{
    public class AgentWorld
    {
        private const string wd = "C:\\code\\Runic\\Agent\\src\\Runic.Agent.UnitTest\\bin\\Debug\\netcoreapp1.0";
        public IPluginManager PluginManager { get; set; }
        public IMessagingService MessagingService { get; set; }
        public IFlowManager FlowManager { get; set; }
        public IStats Stats { get; set; }
        public IDataService DataService { get; set; }
        public IAgentService Agent { get; set; }

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
            var container = new TestStartup().BuildContainer();
            PluginManager = container.Resolve<IPluginManager>();
            FlowManager = container.Resolve<IFlowManager>();
            Stats = container.Resolve<IStats>();
            DataService = container.Resolve<IDataService>();
            MessagingService = new Mock<IMessagingService>().Object;
            Agent = container.Resolve<IAgentService>();
        }
    }
}
