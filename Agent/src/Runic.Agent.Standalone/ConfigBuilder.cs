using Fclp;
using System;

namespace Runic.Agent.Standalone
{
    public interface IConfigBuilder
    {
        Configuration Config { get; }
    }

    public class ConfigBuilder : IConfigBuilder
    {
        private readonly Configuration _config;
        public ConfigBuilder(string[] args)
        {
            _config = GetConfig(args);
        }

        public Configuration Config => _config;

        private Configuration GetConfig(string[] args)
        {
            var configBuilder = GetConfigurationBuilder();
            var configResult = configBuilder.Parse(args);
            if (configResult.HasErrors)
            {
                throw new ArgumentException(configResult.ErrorText);
            }
            return configBuilder.Object;
        }

        private FluentCommandLineParser<Configuration> GetConfigurationBuilder()
        {
            var p = new FluentCommandLineParser<Configuration>();
            p.Setup(arg => arg.PluginFolderPath)
             .As('p', "pluginpath")
             .Required();
            p.Setup(arg => arg.TestPlanPath)
             .As('t', "testplanpath")
             .Required();
            return p;
        }
    }
}
