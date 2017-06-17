using Microsoft.Extensions.Configuration;
using NLog;
using System;

namespace Runic.Agent.Configuration
{
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
