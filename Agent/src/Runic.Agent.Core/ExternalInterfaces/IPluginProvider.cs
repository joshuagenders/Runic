namespace Runic.Agent.Core.ExternalInterfaces
{
    public interface IPluginProvider
    {
        void RetrieveSourceDll(string key);
        string GetFilepath(string key);
    }
}