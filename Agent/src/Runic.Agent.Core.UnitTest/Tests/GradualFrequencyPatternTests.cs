using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Patterns;
using Runic.Agent.TestHarness.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class GradualFrequencyPatternTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGradualWithNoRampUpDownExecutes_CorrectThreadLevelsReturned()
        {
            var pattern = new GradualFrequencyPattern()
            {
                DurationSeconds = 3,
                JourneysPerMinute = 1
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertFrequencyLevel(pattern, startTime, 2, 1);
            AssertFrequencyLevel(pattern, startTime, 4, 0);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGradualWithRampUpDownEdgeAndIntervalCollisionExecutes_CorrectThreadLevelsReturned()
        {
            var pattern = new GradualFrequencyPattern()
            {
                DurationSeconds = 14,
                RampDownSeconds = 7,
                RampUpSeconds = 7,
                JourneysPerMinute = 2
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertFrequencyLevel(pattern, startTime, 1, (1.0/7*2));
            AssertFrequencyLevel(pattern, startTime, 7, 2);
            AssertFrequencyLevel(pattern, startTime, 15, 0);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenAComplexGradualFlowIsExecuted_CorrectThreadLevelsReturned()
        {
            var pattern = new GradualFrequencyPattern()
            {
                DurationSeconds = 14,
                RampDownSeconds = 5,
                RampUpSeconds = 5,
                JourneysPerMinute = 4
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);

            AssertFrequencyLevel(pattern, startTime, 1, 0.8);
            AssertFrequencyLevel(pattern, startTime, 2, 1.6);
            AssertFrequencyLevel(pattern, startTime, 4, 3.2);
            AssertFrequencyLevel(pattern, startTime, 5, 4);
            AssertFrequencyLevel(pattern, startTime, 11, 2.4);
            AssertFrequencyLevel(pattern, startTime, 16, 0);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGradualFlowExecutes_CorrectThreadLevelsReturned()
        {
            var pattern = new GradualFrequencyPattern()
            {
                DurationSeconds = 60,
                RampDownSeconds = 9,
                RampUpSeconds = 9,
                JourneysPerMinute = 3
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertFrequencyLevel(pattern, startTime, 3, 1);
            AssertFrequencyLevel(pattern, startTime, 9, 3);
            AssertFrequencyLevel(pattern, startTime, 10, 3);
            
            AssertFrequencyLevel(pattern, startTime, 30, 3);
            
            AssertFrequencyLevel(pattern, startTime, 50, 3);
            AssertFrequencyLevel(pattern, startTime, 54, 2);            
            AssertFrequencyLevel(pattern, startTime, 62, 0);
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

        private void AssertFrequencyLevel(IFrequencyPattern pattern, DateTime startTime, int secondsEllapsed, double expectedFrequencyLevel)
        {
            Assert.AreEqual(
                expectedFrequencyLevel, 
                pattern.GetCurrentFrequencyPerMinute(
                    startTime, 
                    startTime.AddSeconds(secondsEllapsed)));
        }
    }
}
