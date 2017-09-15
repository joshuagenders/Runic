namespace Runic.Agent.Core.Configuration
{
    public interface IConfiguration
    {
        int MaxThreads { get; }
        int MaxErrors { get; }
    }
}
