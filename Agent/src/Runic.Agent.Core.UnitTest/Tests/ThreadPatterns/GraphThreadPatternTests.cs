using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.ThreadPatterns
{
    [TestClass]
    public class GraphThreadPatternTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenExpandingGraphPatternExecuted_CorrectThreadLevelsReturned()
        {
            var pattern = new GraphThreadPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 60,
                Points = new List<Point>()
                {
                   new Point(){ threadLevel = 2, unitsFromStart = 0 },
                   new Point(){ threadLevel = 5, unitsFromStart = 1 },
                   new Point(){ threadLevel = 0, unitsFromStart = 2 }
                }
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertThreadLevel(pattern, startTime, 1, 2);
            AssertThreadLevel(pattern, startTime, 31, 5);
            AssertThreadLevel(pattern, startTime, 61, 0);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenShrinkingGraphExecutes_CorrectThreadLevelsReturned()
        {
            var pattern = new GraphThreadPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 20,
                Points = new List<Point>()
                {
                   new Point(){ threadLevel = 2, unitsFromStart = 0 },
                   new Point(){ threadLevel = 5, unitsFromStart = 50 },
                   new Point(){ threadLevel = 0, unitsFromStart = 100 }
                }
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertThreadLevel(pattern, startTime, 1, 2);
            AssertThreadLevel(pattern, startTime, 11, 5);
            AssertThreadLevel(pattern, startTime, 21, 0);
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

        private void AssertThreadLevel(IThreadPattern pattern, DateTime startTime, int secondsEllapsed, int expectedThreadLevel)
        {
            _mockDatetimeService.Setup(s => s.Now).Returns(startTime.AddSeconds(secondsEllapsed));
            Assert.AreEqual(expectedThreadLevel, pattern.GetCurrentThreadLevel(startTime));
        }
    }
}
