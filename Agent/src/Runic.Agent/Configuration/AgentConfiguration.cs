using System;
using Microsoft.Extensions.Configuration;
using System.IO;

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
