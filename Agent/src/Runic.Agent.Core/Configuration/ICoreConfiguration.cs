namespace Runic.Agent.Core.Configuration
{
    public interface ICoreConfiguration
    {
        int MaxActivePopulation { get; }
        int MaxErrors { get; }
        int StepTimeoutSeconds { get; }
        int TaskCreationPollingIntervalSeconds { get; }
        int PopulationRequestTimeoutSeconds { get; }
        int JourneyTimeoutSeconds { get; }
    }
    //use observer
}
