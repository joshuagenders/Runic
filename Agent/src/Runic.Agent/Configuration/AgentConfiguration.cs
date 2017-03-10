using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using NLog;

namespace Runic.Agent.Configuration
{
    public class AgentConfiguration
    {
        private IConfigurationRoot Configuration { get; set; }
        private static AgentConfiguration _instance { get; set; }
        public static AgentConfiguration Instance => _instance ?? (_instance = new AgentConfiguration());

        private AgentConfiguration(){}

        public static void LoadConfiguration(string[] args = null)
        {
            var builder = new ConfigurationBuilder();
            if (args != null)
                builder.AddCommandLine(args);

            builder.SetBasePath(Directory.GetCurrentDirectory());
            if (File.Exists("appsettings.json"))
                builder.AddJsonFile("appsettings.json");

            _instance = new AgentConfiguration();
            Instance.Configuration = builder.Build();
        }

        public int MaxThreads => Configuration.GetConfigValue("Agent:MaxThreads", int.Parse, 0);
        public int LifetimeSeconds => Configuration.GetConfigValue("Agent:LifetimeSeconds", int.Parse, 60);
        public string ClientConnectionConfiguration => Configuration.GetConfigValue("Client:MQConnectionString", "");
        public int StatsdPort => Configuration.GetConfigValue("Statsd:Port", int.Parse, 8125);
        public string StatsdHost => Configuration.GetConfigValue("Statsd:Host", "localhost");
        public string StatsdPrefix => Configuration.GetConfigValue("Statsd:Prefix", "Runic.Agent.");
    }

    public static class ConfigurationExtensions
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        public static T GetValueFromAppSettings<T>(
            this IConfigurationRoot configuration, string name, Func<string, T> parser, T defaultValue)
        {
            if (name == null)
            {
                _logger.Error("Attempted to retrieve configuration key but was passed a null key");
                return default(T);
            }
            if (parser == null)
            {
                _logger.Error("Attempted to retrieve configuration key but was passed a null parser");
                return default(T);
            }

            var value = configuration[name];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : parser(value);
        }

        public static string GetValueFromAppSettings(this IConfigurationRoot configuration, string name, string defaultValue)
        {
            if (name == null)
            {
                _logger.Error("Attempted to retrieve configuration key but was passed a null key");
                return null;
            }

            var value = configuration[name];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        public static T GetValueFromEnvironmentVariables<T>(this IConfigurationRoot configuration, string name, Func<string, T> parser, T defaultValue)
        {
            if (name == null)
            {
                _logger.Error("Attempted to retrieve configuration key but was passed a null key");
                return default(T);
            }
            if (parser == null)
            {
                _logger.Error("Attempted to retrieve configuration key but was passed a null parser");
                return default(T);
            }

            var value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : parser(value);
        }

        public static string GetValueFromEnvironmentVariables(string name, string defaultValue)
        {
            if (name == null)
            {
                _logger.Error("Attempted to retrieve configuration key but was passed a null key");
                return defaultValue;
            }

            var value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        public static string GetConfigValue(this IConfigurationRoot configuration, string name, string defaultValue)
        {
            if (name == null)
            {
                _logger.Error("Attempted to retrieve configuration key but was passed a null key");
                return defaultValue;
            }
            
            var value = Environment.GetEnvironmentVariable(name);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            value = configuration[name];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        public static T GetConfigValue<T>(this IConfigurationRoot configuration, string name, Func<string, T> parser, T defaultValue)
        {
            if (name == null)
            {
                _logger.Error("Attempted to retrieve configuration key but was passed a null key");
                return defaultValue;
            }
            if (parser == null)
            {
                _logger.Error("Attempted to retrieve configuration key but was passed a null parser");
                return defaultValue;
            }

            var value = Environment.GetEnvironmentVariable(name);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return parser(value);
            }
            value = configuration[name];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : parser(value);
        }
    }
}