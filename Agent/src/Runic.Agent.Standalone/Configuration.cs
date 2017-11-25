using Runic.Agent.Core.Configuration;
using System;

namespace Runic.Agent.Standalone
{
    public class Configuration : ICoreConfiguration
    {
        public int TaskCreationPollingIntervalSeconds => throw new NotImplementedException();

        public string PluginFolderPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
