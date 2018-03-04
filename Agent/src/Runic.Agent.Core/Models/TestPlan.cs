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
            Frequency = frequency;
        }
        
        public Journey Journey { get; }
        public FrequencyPattern Frequency { get; }
    }
}
