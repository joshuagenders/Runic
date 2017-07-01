namespace Runic.Agent.Core.AssemblyManagement
{
    public interface IPluginProvider
    {
        void RetrieveSourceDll(string key);
        string GetFilepath(string key);
    }
}