namespace Runic.Agent.Framework.ExternalInterfaces
{
    public interface IPluginProvider
    {
        void RetrieveSourceDll(string key);
        string GetFilepath(string key);
    }
}