using Runic.Agent.Core.Models;
using System;

namespace Runic.Agent.Core.WorkGenerator
{
    public class TestPlan
    {
        public TestPlan(
            DateTime startTime, 
            Journey journey, 
            double journeysPerMinute, 
            FrequencyPattern frequency)
        {
            StartTime = startTime;
            Journey = journey;
            JourneysPerMinute = journeysPerMinute;
            Frequency = frequency;
        }

        public DateTime StartTime { get; }
        public Journey Journey { get; }
        public double JourneysPerMinute { get; }
        public FrequencyPattern Frequency { get; }
    }
}
