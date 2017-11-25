namespace Runic.Agent.Core.Configuration
{
    public interface ICoreConfiguration
    {
        int TaskCreationPollingIntervalSeconds { get; }
        string PluginFolderPath { get; set; }
    }
}
