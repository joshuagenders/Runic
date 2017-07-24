namespace Runic.Agent.Standalone.Configuration
{
    public interface IStatsdSettings
    {
        string Prefix { get; }
        int Port { get; }
        string Host { get; }
    }
}
