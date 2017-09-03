using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadPatterns;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.ThreadPatterns
{
    [TestClass]
    public class ConstantThreadPatternTests
    {

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenConstantThreadPatternIsExecuted_CorrectThreadLevelsReturned()
        {
            var pattern = new ConstantThreadPattern(_mockDatetimeService.Object)
            {
                ThreadCount = 4
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertThreadLevel(pattern, startTime, 1, 4);
            AssertThreadLevel(pattern, startTime, 5425, 4);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenConstantPatternWithDurationIsExecuted_CorrectThreadLevelsReturned()
        {
            var pattern = new ConstantThreadPattern(_mockDatetimeService.Object)
            {
                ThreadCount = 4,
                DurationSeconds = 20
            };
            var startTime = new DateTime(2017, 1, 1, 1, 1, 0);
            AssertThreadLevel(pattern, startTime, 1, 4);
            AssertThreadLevel(pattern, startTime, 19, 4);
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