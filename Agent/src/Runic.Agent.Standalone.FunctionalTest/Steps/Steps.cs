using FluentAssertions;

namespace Runic.Agent.FunctionalTest.Steps
{
    public class Steps 
    {
        public Steps GivenIHaveAJourneyFor(string journeyKey)
        {
            return this;
        }

        public Steps GivenIHaveAFrequencyPattern(string patternType)
        {
            return this;
        }

        public Steps GivenTheJourneyLastsForSeconds(int journeyLengthSeconds)
        {
            return this;
        }

        public Steps GivenTheFrequencyIsJourneysPerMinute(int journeysPerMinute)
        {
            return this;
        }

        public Steps WhenIEmbarkOnTheJourney()
        {
            return this;
        }

        public Steps ThenThePageIsReturnedAtleastTimes(string pageName, int times)
        {
            return this;
        }
    }
}
