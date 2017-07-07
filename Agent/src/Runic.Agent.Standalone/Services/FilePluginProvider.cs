using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Standalone.Configuration;
using System.IO;

namespace Runic.Agent.Standalone.Services
{
    public class FilePluginProvider : IPluginProvider
    {
        public FilePluginProvider(string folderPath)
        {
            FolderPath = folderPath;
        }

        private string FolderPath { get; }

        public string GetFilepath(string key)
        {
            return Path.Combine(FolderPath ?? Directory.GetCurrentDirectory(), AgentConfig.AgentSettings.PluginDirectory, $"{key}.dll");
        }

        public void RetrieveSourceDll(string key)
        {
        }
    }
}