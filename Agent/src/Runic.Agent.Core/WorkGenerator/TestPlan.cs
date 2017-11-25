using Runic.Agent.Core.Patterns;
using Runic.Agent.Core.Models;
using System;

namespace Runic.Agent.Core.WorkGenerator
{
    public class TestPlan
    {
        public TestPlan(Journey journey)
        {
            Journey = journey;
        }

        public DateTime StartTime { get; set; }
        public Journey Journey { get; private set; }
        public double JourneysPerMinute { get; set; }
        public IFrequencyPattern Frequency { get; set; }
    }
}
