using FluentAssertions;
using Runic.Agent.Core.Patterns;
using System;
using Xunit;

namespace Runic.Agent.Core.UnitTest.Tests
{
    public class GradualFrequencyPatternTests
    {
        [Theory]
        [InlineData(-1, 0)]
        [InlineData(2,1)]
        [InlineData(3, 1)]
        [InlineData(4,0)]
        public void WhenGradualWithNoRampUpDownExecutes_CorrectThreadLevelsReturned(int time, int value)
        {
            var pattern = new GradualFrequencyPattern()
            {
                DurationSeconds = 3,
                JourneysPerMinute = 1
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertFrequencyLevel(pattern, startTime, time, value);
        }

        //[Theory]
        //[InlineData(-1, 0)]
        //[InlineData(1, 1.0/7*2)]
        //[InlineData(7.5, 2)]
        //[InlineData(17, 0)]
        public void WhenGradualWithRampUpDownEdgeAndIntervalCollisionExecutes_CorrectThreadLevelsReturned(int time, int value)
        {
            var pattern = new GradualFrequencyPattern()
            {
                DurationSeconds = 14,
                RampDownSeconds = 7,
                RampUpSeconds = 7,
                JourneysPerMinute = 2
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertFrequencyLevel(pattern, startTime, time, value);
        }


        [Theory]
        //[InlineData(1, 0.8)]
        //[InlineData(2, 2.4)]
        //[InlineData(4, 3.6)]
        //[InlineData(5, 4)]
        //[InlineData(11, 2.4)]
        [InlineData(15, 0)]
        public void WhenAComplexGradualFlowIsExecuted_CorrectThreadLevelsReturned(int time, int value)
        {
            var pattern = new GradualFrequencyPattern()
            {
                DurationSeconds = 14,
                RampDownSeconds = 5,
                RampUpSeconds = 5,
                JourneysPerMinute = 4
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);

            AssertFrequencyLevel(pattern, startTime, time, value);
        }
        
        [Theory]
        [InlineData(3,1)]
        [InlineData(6, 2)]
        [InlineData(9, 3)]
        [InlineData(10, 3)]
        [InlineData(30, 3)]
        [InlineData(50, 3)]
        [InlineData(54, 2)]
        [InlineData(60, 0)]
        [InlineData(61, 0)]
        [InlineData(62, 0)]
        public void WhenGradualFlowExecutes_CorrectThreadLevelsReturned(int time, int value)
        {
            var pattern = new GradualFrequencyPattern()
            {
                DurationSeconds = 60,
                RampDownSeconds = 9,
                RampUpSeconds = 9,
                JourneysPerMinute = 3
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertFrequencyLevel(pattern, startTime, time, value);
        }

        private void AssertFrequencyLevel(IFrequencyPattern pattern, DateTime startTime, int secondsEllapsed, double expectedFrequencyLevel)
        {
            pattern.GetCurrentFrequencyPerMinute(startTime, startTime.AddSeconds(secondsEllapsed))
                   .Should()
                   .Be(expectedFrequencyLevel);
        }
    }
}
