using System.IO;
using Microsoft.Extensions.Configuration;

namespace Runic.Agent.Configuration
{
    public class AgentConfiguration
    {
        private IConfigurationRoot Configuration { get; set; }
        private static AgentConfiguration _instance { get; set; }
        public static AgentConfiguration Instance => _instance ?? (_instance = new AgentConfiguration());

        public int MaxThreads => Configuration.GetConfigValue("Agent:MaxThreads", int.Parse, 0);
        public int LifetimeSeconds => Configuration.GetConfigValue("Agent:LifetimeSeconds", int.Parse, 60);
        public string ClientConnectionConfiguration => Configuration.GetConfigValue("Client:MQConnectionString", "");
        public int StatsdPort => Configuration.GetConfigValue("Statsd:Port", int.Parse, 8125);
        public string StatsdHost => Configuration.GetConfigValue("Statsd:Host", "localhost");
        public string StatsdPrefix => Configuration.GetConfigValue("Statsd:Prefix", "Runic.Agent.");

        private AgentConfiguration() { }

        public static void LoadConfiguration(string[] args = null)
        {
            var builder = new ConfigurationBuilder();
            if (args != null)
                builder.AddCommandLine(args);

            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", true);

            _instance = new AgentConfiguration();
            Instance.Configuration = builder.Build();
        }
    }
}