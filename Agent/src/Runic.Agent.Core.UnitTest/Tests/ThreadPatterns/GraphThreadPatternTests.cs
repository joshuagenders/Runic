using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
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
            var pattern = new GraphPopulationPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 60,
                Points = new List<Point>()
                {
                   new Point(){ PopulationSize = 2, UnitsFromStart = 0 },
                   new Point(){ PopulationSize = 5, UnitsFromStart = 1 },
                   new Point(){ PopulationSize = 0, UnitsFromStart = 2 }
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
            var pattern = new GraphPopulationPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 20,
                Points = new List<Point>()
                {
                   new Point(){ PopulationSize = 2, UnitsFromStart = 0 },
                   new Point(){ PopulationSize = 5, UnitsFromStart = 50 },
                   new Point(){ PopulationSize = 0, UnitsFromStart = 100 }
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

        private void AssertThreadLevel(IPopulationPattern pattern, DateTime startTime, int secondsEllapsed, int expectedThreadLevel)
        {
            _mockDatetimeService.Setup(s => s.Now).Returns(startTime.AddSeconds(secondsEllapsed));
            Assert.AreEqual(expectedThreadLevel, pattern.GetCurrentActivePopulationCount(startTime));
        }
    }
}
