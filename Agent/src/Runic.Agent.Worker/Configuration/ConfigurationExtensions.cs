using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Runic.Agent.Worker.Configuration
{
    public static class ConfigurationExtensions
    {
        private static readonly ILogger _logger = new LoggerFactory().CreateLogger(nameof(ConfigurationExtensions));

        public static ContainerBuilder Register<T,U>(this ContainerBuilder builder)
        {
            builder.RegisterType<T>().As<U>();
            return builder;
        }

        public static T GetValueFromAppSettings<T>(
            this IConfigurationRoot configuration, string name, Func<string, T> parser, T defaultValue)
        {
            if (name == null)
            {
                _logger.LogError("Attempted to retrieve configuration key but was passed a null key");
                return default(T);
            }
            if (parser == null)
            {
                _logger.LogError("Attempted to retrieve configuration key but was passed a null parser");
                return default(T);
            }

            var value = configuration[name];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : parser(value);
        }

        public static string GetValueFromAppSettings(this IConfigurationRoot configuration, string name, string defaultValue)
        {
            if (name == null)
            {
                _logger.LogError("Attempted to retrieve configuration key but was passed a null key");
                return null;
            }

            var value = configuration[name];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        public static T GetValueFromEnvironmentVariables<T>(this IConfigurationRoot configuration, string name, Func<string, T> parser, T defaultValue)
        {
            if (name == null)
            {
                _logger.LogError("Attempted to retrieve configuration key but was passed a null key");
                return default(T);
            }
            if (parser == null)
            {
                _logger.LogError("Attempted to retrieve configuration key but was passed a null parser");
                return default(T);
            }

            var value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : parser(value);
        }

        public static string GetValueFromEnvironmentVariables(string name, string defaultValue)
        {
            if (name == null)
            {
                _logger.LogError("Attempted to retrieve configuration key but was passed a null key");
                return defaultValue;
            }

            var value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        public static string GetConfigValue(this IConfigurationRoot configuration, string name, string defaultValue)
        {
            if (name == null)
            {
                _logger.LogError("Attempted to retrieve configuration key but was passed a null key");
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
                _logger.LogError("Attempted to retrieve configuration key but was passed a null key");
                return defaultValue;
            }
            if (parser == null)
            {
                _logger.LogError("Attempted to retrieve configuration key but was passed a null parser");
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
