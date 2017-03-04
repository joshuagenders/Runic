using System.Collections.Generic;
using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;
using StatsN;

namespace Runic.Configuration
{
    public static class RunicConfiguration
    {
        private static IConfigurationRoot Configuration { get; set; }
        public static IContainer Container { get; set; }

        public static StatsdOptions GetStatsdConfiguration => new StatsdOptions
        {
            HostOrIp = Configuration["Database:Host"],
            Port = int.Parse(Configuration["Database:Port"]),
            Prefix = Configuration["Database:Prefix"]
        };

        public static string ClientConnectionConfiguration => Configuration["Client:MQConnectionString"];

        public static void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static void BuildContainer()
        {
            var containerBuilder = new ContainerBuilder();
            Container = containerBuilder.Build();
        }

        public static void BuildConfiguration(IEnumerable<KeyValuePair<string, string>> settings)
        {
            var builder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            Configuration = builder.Build();
        }
    }
}