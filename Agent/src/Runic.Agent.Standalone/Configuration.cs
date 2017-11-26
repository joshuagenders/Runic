using Runic.Agent.Core.Configuration;
using System;

namespace Runic.Agent.Standalone
{
    public class Configuration : ICoreConfiguration
    {
        public int TaskCreationPollingIntervalSeconds { get; set; }
        public string PluginFolderPath { get; set; }
        public string WorkFolderPath { get; internal set; }
    }
}
