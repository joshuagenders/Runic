using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Agent.TestHarness.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.ThreadPatterns
{
    [TestClass]
    public class GradualThreadPatternTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGradualWithNoRampUpDownExecutes__CorrectThreadLevelsReturned()
        {
            var pattern = new GradualThreadPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 2,
                ThreadCount = 2
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertThreadLevel(pattern, startTime, 2, 2);
            AssertThreadLevel(pattern, startTime, 4, 0);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGradualWithRampUpDownEdgeAndIntervalCollisionExecutes_CorrectThreadLevelsReturned()
        {
            var pattern = new GradualThreadPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 14,
                RampDownSeconds = 7,
                RampUpSeconds = 7,
                ThreadCount = 2
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertThreadLevel(pattern, startTime, 0, 1);
            AssertThreadLevel(pattern, startTime, 7, 2);
            AssertThreadLevel(pattern, startTime, 15, 0);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenAComplexGradualFlowIsExecuted_CorrectThreadLevelsReturned()
        {
            var pattern = new GradualThreadPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 14,
                RampDownSeconds = 5,
                RampUpSeconds = 5,
                ThreadCount = 4
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);

            AssertThreadLevel(pattern, startTime, 1, 1);
            AssertThreadLevel(pattern, startTime, 3, 2);
            AssertThreadLevel(pattern, startTime, 4, 3);
            AssertThreadLevel(pattern, startTime, 5, 4);
            AssertThreadLevel(pattern, startTime, 11, 3);
            AssertThreadLevel(pattern, startTime, 12, 2);
            AssertThreadLevel(pattern, startTime, 13, 1);
            AssertThreadLevel(pattern, startTime, 15, 0);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGradualFlowExecutes_CorrectThreadLevelsReturned()
        {
            var pattern = new GradualThreadPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 60,
                RampDownSeconds = 9,
                RampUpSeconds = 9,
                ThreadCount = 3
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertThreadLevel(pattern, startTime, 1, 1);
            AssertThreadLevel(pattern, startTime, 8, 2);
            AssertThreadLevel(pattern, startTime, 10, 3);

            AssertThreadLevel(pattern, startTime, 30, 3);

            AssertThreadLevel(pattern, startTime, 50, 3);
            AssertThreadLevel(pattern, startTime, 54, 2);
            AssertThreadLevel(pattern, startTime, 58, 1);

            AssertThreadLevel(pattern, startTime, 62, 0);
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
