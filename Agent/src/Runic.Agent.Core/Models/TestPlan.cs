namespace Runic.Agent.Core.Models
{
    public class TestPlan
    {
        public TestPlan(
            Journey journey, 
            double journeysPerMinute, 
            FrequencyPattern frequency)
        {
            Journey = journey;
            JourneysPerMinute = journeysPerMinute;
            Frequency = frequency;
        }
        
        public Journey Journey { get; }
        public double JourneysPerMinute { get; }
        public FrequencyPattern Frequency { get; }
    }
}
