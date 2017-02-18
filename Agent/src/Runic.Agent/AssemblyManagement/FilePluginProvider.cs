using System.IO;

namespace Runic.Agent.AssemblyManagement
{
    public class FilePluginProvider : IPluginProvider
    {
        private string _folderPath { get; set; }
        public FilePluginProvider(string folderPath)
        {
            _folderPath = folderPath;
        }

        public string GetFilepath(string key)
        {
            return Path.Combine(_folderPath, $"{key}.dll");
        }

        public void RetrieveSourceDll(string key)
        {
        }
    }
}
