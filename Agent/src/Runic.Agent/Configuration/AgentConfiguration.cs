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

        public static void LoadConfiguration(string[] args)
        {
            var builder = new ConfigurationBuilder();

            builder.AddCommandLine(args);
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            _logger.Info($"args:{args.ToList().Select(t => $"| {t} ")}");
            _logger.Info($"MaxThreads:{MaxThreads}");
            _logger.Info($"LifetimeSeconds:{LifetimeSeconds}");
        }
    }
}