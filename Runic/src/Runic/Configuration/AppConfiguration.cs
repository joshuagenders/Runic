using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using StatsN;

namespace Runic.Configuration
{
    public class AppConfiguration
    {
        public static IConfigurationRoot Configuration { get; set; }
        
        public static void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static void BuildConfiguration(IEnumerable<KeyValuePair<string,string>> settings)
        {
            var builder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            Configuration = builder.Build();
        }

        public static StatsdOptions GetStatsdConfiguration {
            get
            {
                return new StatsdOptions()
                {
                    HostOrIp = Configuration["Database:Host"],
                    Port = int.Parse(Configuration["Database:Port"]),
                    Prefix = Configuration["Database:Prefix"]
                };
            }
        }
    }
}
