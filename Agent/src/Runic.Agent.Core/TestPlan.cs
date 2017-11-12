using Runic.Agent.Core.Patterns;
using Runic.Agent.Framework.Models;
using System;

namespace Runic.Agent.Core
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
