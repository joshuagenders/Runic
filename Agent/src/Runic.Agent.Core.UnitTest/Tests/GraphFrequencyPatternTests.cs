using Moq;
using Runic.Agent.Core.Patterns;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Runic.Agent.Core.UnitTest.Tests
{
    
    public class GraphFrequencyPatternTests
    {
        private Mock<IDatetimeService> _mockDatetimeService { get; set; }
        private SemaphoreSlim _semaphore { get; set; }

        public GraphFrequencyPatternTests()
        {
            _mockDatetimeService = new Mock<IDatetimeService>();
            _semaphore = new SemaphoreSlim(0);
            _mockDatetimeService
                .Setup(d => d.WaitMilliseconds(
                            It.IsAny<int>(),
                            It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() =>
                {
                    _semaphore.Wait();
                }));
            _mockDatetimeService.Setup(d => d.Now).Returns(DateTime.Now);
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(1,2)]
        [InlineData(31,5)]
        [InlineData(61,0)]
        public void WhenExpandingGraphPatternExecuted_CorrectThreadLevelsReturned(int time, int value)
        {
            var pattern = new GraphFrequencyPattern()
            {
                DurationSeconds = 60,
                Points = new List<Point>()
                {
                   new Point(){ FrequencyPerMinute = 2, UnitsFromStart = 0 },
                   new Point(){ FrequencyPerMinute = 5, UnitsFromStart = 1 },
                   new Point(){ FrequencyPerMinute = 0, UnitsFromStart = 2 }
                }
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertFrequencyLevel(pattern, startTime, time, value);
        }


        [Theory]
        [InlineData(-1, 0)]
        [InlineData(1, 2)]
        [InlineData(11, 5)]
        [InlineData(21, 0)]
        public void WhenShrinkingGraphExecutes_CorrectThreadLevelsReturned(int time, int value)
        {
            var pattern = new GraphFrequencyPattern()
            {
                DurationSeconds = 20,
                Points = new List<Point>()
                {
                   new Point(){ FrequencyPerMinute = 2, UnitsFromStart = 0 },
                   new Point(){ FrequencyPerMinute = 5, UnitsFromStart = 50 },
                   new Point(){ FrequencyPerMinute = 0, UnitsFromStart = 100 }
                }
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
