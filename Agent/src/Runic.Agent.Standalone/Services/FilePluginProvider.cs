using Runic.Agent.Core.AssemblyManagement;
using System.IO;

namespace Runic.Agent.Standalone.Services
{
    public class FilePluginProvider : IPluginProvider
    {
        public FilePluginProvider(string folderPath, string pluginDirectory)
        {
            _folderPath = folderPath;
            _pluginDirectory = pluginDirectory;
        }

        private string _folderPath { get; }
        private string _pluginDirectory { get; }

        public string GetFilepath(string key)
        {
            return Path.Combine(
                _folderPath ?? Directory.GetCurrentDirectory(), 
                _pluginDirectory ?? "", 
                $"{key}.dll");
        }

        public void RetrieveSourceDll(string key)
        {
        }
    }
}