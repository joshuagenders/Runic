using System.IO;
using Microsoft.Extensions.Configuration;

namespace Runic.Agent.Configuration
{
    public class AgentConfiguration
    {
        public static IConfigurationRoot Config { get; set; }

        public static void LoadConfiguration(string[] args)
        {
            var builder = new ConfigurationBuilder();

            builder.AddCommandLine(args);
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");

            Config = builder.Build();
        }
    }
}