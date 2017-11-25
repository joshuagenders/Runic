using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Patterns;
using Runic.Agent.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class ConstantFrequencyPatternTests
    {

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenConstantThreadPatternIsExecuted_CorrectThreadLevelsReturned()
        {
            var pattern = new ConstantFrequencyPattern()
            {
                JourneysPerMinute = 4
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertFrequencyLevel(pattern, startTime, 1, 4);
            AssertFrequencyLevel(pattern, startTime, 5425, 4);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenConstantPatternWithDurationIsExecuted_CorrectThreadLevelsReturned()
        {
            var pattern = new ConstantFrequencyPattern()
            {
                JourneysPerMinute = 4,
                DurationSeconds = 20
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertFrequencyLevel(pattern, startTime, 1, 4);
            AssertFrequencyLevel(pattern, startTime, 19, 4);
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