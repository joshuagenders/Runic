namespace Runic.Agent.Core.Configuration
{
    public interface ICoreConfiguration
    {
        int MaxThreads { get; }
        int MaxErrors { get; }
    }
}
