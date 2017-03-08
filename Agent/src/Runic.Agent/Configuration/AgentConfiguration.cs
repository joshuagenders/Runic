using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NLog;

namespace Runic.Agent.Configuration
{
    public static class AgentConfiguration
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static IConfigurationRoot Configuration { get; set; }

        public static int MaxThreads => int.Parse(Configuration["Agent:MaxThreads"]);
        public static int LifetimeSeconds => int.Parse(Configuration["Agent:LifetimeSeconds"]);
        public static string ClientConnectionConfiguration => Configuration["Client:MQConnectionString"];
        public static int StatsdPort => int.Parse(Configuration["Statsd:Port"]);
        public static string StatsdHost => Configuration["Statsd:Host"];
        public static string StatsdPrefix => Configuration["Statsd:Prefix"];

        public static void LoadConfiguration(string[] args = null)
        {
            var builder = new ConfigurationBuilder();
            if (args != null)
                builder.AddCommandLine(args);

            builder.SetBasePath(Directory.GetCurrentDirectory());
            if (File.Exists("appsettings.json"))
                builder.AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            if (args != null)
                _logger.Debug($"args:{args.ToList().Select(t => $"| {t} ")}");
            _logger.Debug($"MaxThreads:{MaxThreads}");
            _logger.Debug($"LifetimeSeconds:{LifetimeSeconds}");
            _logger.Debug($"StatsdHost:{StatsdHost}");
            _logger.Debug($"StatsdPort:{StatsdPort}");
            _logger.Debug($"StatsdPrefix:{StatsdPrefix}");
            _logger.Debug($"ClientConnectionConfiguration:{ClientConnectionConfiguration}");
        }
    }
}