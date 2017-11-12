using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Patterns;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.ThreadPatterns
{
    [TestClass]
    public class GraphFrequencyPatternTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenExpandingGraphPatternExecuted_CorrectThreadLevelsReturned()
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
            AssertFrequencyLevel(pattern, startTime, 1, 2);
            AssertFrequencyLevel(pattern, startTime, 31, 5);
            AssertFrequencyLevel(pattern, startTime, 61, 0);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenShrinkingGraphExecutes_CorrectThreadLevelsReturned()
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
            AssertFrequencyLevel(pattern, startTime, 1, 2);
            AssertFrequencyLevel(pattern, startTime, 11, 5);
            AssertFrequencyLevel(pattern, startTime, 21, 0);
        }

        private Mock<IDatetimeService> _mockDatetimeService { get; set; }
        private SemaphoreSlim _semaphore { get; set; }

        [TestInitialize]
        public void Init()
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

        private void AssertFrequencyLevel(IFrequencyPattern pattern, DateTime startTime, int secondsEllapsed, int expectedFrequencyLevel)
        {
            Assert.AreEqual(
                expectedFrequencyLevel, 
                pattern.GetCurrentFrequencyPerMinute(
                    startTime, 
                    startTime.AddSeconds(secondsEllapsed)));
        }
    }
}
