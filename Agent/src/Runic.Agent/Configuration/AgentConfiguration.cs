using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NLog;

namespace Runic.Agent.Configuration
{
    public static class AgentConfiguration
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static IConfigurationRoot Config { get; set; }

        public static int MaxThreads => int.Parse(Config["Agent:MaxThreads"]);
        public static int LifetimeSeconds => int.Parse(Config["Agent:LifetimeSeconds"]);
        public static string ClientConnectionConfiguration => Config["Client:MQConnectionString"];

        public static void LoadConfiguration(string[] args)
        {
            var builder = new ConfigurationBuilder();

            builder.AddCommandLine(args);
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");

            Config = builder.Build();

            _logger.Info($"args:{args.ToList().Select(t => $"| {t} ")}");
            _logger.Info($"MaxThreads:{MaxThreads}");
            _logger.Info($"LifetimeSeconds:{LifetimeSeconds}");
        }
    }
}