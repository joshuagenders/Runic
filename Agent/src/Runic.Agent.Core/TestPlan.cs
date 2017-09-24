using Runic.Agent.Framework.Models;
using System;

namespace Runic.Agent.Core
{
    public class TestPlan
    {
        public TestPlan(Journey journey, Population population)
        {
            InitialiseJourneyFrequency(journey);
            Journey = journey;
            Population = population;
        }

        private void InitialiseJourneyFrequency(Journey journey)
        {
            int frequencySeconds = 0;
            if (journey.Frequency.JourneysPerHour != 0)
            {
                frequencySeconds = 3600 / journey.Frequency.JourneysPerHour;
            }
            if (journey.Frequency.JourneysPerMinute != 0)
            {
                frequencySeconds = 60 / journey.Frequency.JourneysPerMinute;
            }
            JourneyFrequencySeconds = frequencySeconds;
        }

        public DateTime StartTime { get; set; }
        public Journey Journey { get; private set; }
        public Population Population { get; private set; }
        public int JourneyFrequencySeconds { get; private set; }
    }
}
