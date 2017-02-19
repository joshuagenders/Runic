namespace Runic.Agent.AssemblyManagement
{
    public interface IPluginProvider
    {
        void RetrieveSourceDll(string key);
        string GetFilepath(string key);
    }
}