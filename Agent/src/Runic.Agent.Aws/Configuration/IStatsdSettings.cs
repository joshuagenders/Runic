namespace Runic.Agent.Aws.Configuration
{
    public interface IStatsdSettings
    {
        string Prefix { get; }
        int Port { get; }
        string Host { get; }
    }
}
