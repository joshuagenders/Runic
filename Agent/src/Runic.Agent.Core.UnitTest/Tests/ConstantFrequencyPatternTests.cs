using FluentAssertions;
using Runic.Agent.Core.Patterns;
using System;
using Xunit;

namespace Runic.Agent.Core.UnitTest.Tests
{
    public class ConstantFrequencyPatternTests
    {   
        [Theory]
        [InlineData(1, 4)]
        [InlineData(5342, 4)]
        [InlineData(999999, 4)]
        [InlineData(-100, 0)]
        public void WhenConstantThreadPatternIsExecuted_CorrectThreadLevelsReturned(int time, int value)
        {
            var pattern = new ConstantFrequencyPattern()
            {
                JourneysPerMinute = 4
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertFrequencyLevel(pattern, startTime, time, value);
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(1, 4)]
        [InlineData(19, 4)]
        [InlineData(20, 4)]
        [InlineData(21, 0)]
        public void WhenConstantPatternWithDurationIsExecuted_CorrectThreadLevelsReturned(int time, int value)
        {
            var pattern = new ConstantFrequencyPattern()
            {
                JourneysPerMinute = 4,
                DurationSeconds = 20
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertFrequencyLevel(pattern, startTime, time, value);
        }

        private void AssertFrequencyLevel(IFrequencyPattern pattern, DateTime startTime, int secondsEllapsed, int expectedFrequencyLevel)
        { 
            pattern.GetCurrentFrequencyPerMinute(startTime, startTime.AddSeconds(secondsEllapsed))
                   .Should()
                   .Be(expectedFrequencyLevel);
        }
    }
}