using Xunit;

namespace Runic.Agent.FunctionalTest.Features
{
    public class ConstantFrequencyFeature
    {
        [Theory]
        [InlineData(6, 10, 1)]
        [InlineData(60, 61, 60)]
        [InlineData(0, 10, 0)]
        public void GoogleConstantFeature_PageLoadCounts(int journeysPerMinute, int journeyLength, int numberOfTimes)
        {
            new Steps.Steps()
                     .GivenIHaveAJourneyFor("Google")
                     .GivenIHaveAFrequencyPattern("constant")
                     .GivenTheFrequencyIsJourneysPerMinute(journeysPerMinute)
                     .GivenTheJourneyLastsForSeconds(journeyLength)
                     .WhenIEmbarkOnTheJourney()
                     .ThenThePageIsReturnedAtleastTimes("Google", numberOfTimes);
        }
    }
}
