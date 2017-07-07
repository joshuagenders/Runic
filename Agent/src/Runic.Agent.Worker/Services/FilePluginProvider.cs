using Runic.Agent.Core.AssemblyManagement;
using System.IO;

namespace Runic.Agent.Worker.Services
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
            return Path.Combine(FolderPath ?? Directory.GetCurrentDirectory(), "Plugins", $"{key}.dll");
        }

        public void RetrieveSourceDll(string key)
        {
        }
    }
}