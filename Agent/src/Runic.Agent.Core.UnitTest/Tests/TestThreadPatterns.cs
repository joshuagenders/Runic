using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Agent.Core.Services.Interfaces;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestThreadPatterns
    {
        private Mock<IDatetimeService> _mockDatetimeService { get; set; }
        private SemaphoreSlim _semaphore { get; set; }

        [TestInitialize]
        public void Init()
        {
            _mockDatetimeService = new Mock<IDatetimeService>();
            _semaphore = new SemaphoreSlim(0);
            _mockDatetimeService
                .Setup(d => d.WaitUntil(
                            It.IsAny<int>(), 
                            It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() =>
                {
                    _semaphore.Wait();
                }));
            _mockDatetimeService.Setup(d => d.Now).Returns(DateTime.Now);
        }

        private async Task <List<int>> InvokeThreadPattern(IThreadPattern threadPattern)
        {
            var calls = new List<int>();
            threadPattern.RegisterThreadChangeHandler((threadCount) =>
            {
                calls.Add(threadCount);
            });

            var cts = new CancellationTokenSource();
            var maxDuration = threadPattern.GetMaxDurationSeconds();
            if (maxDuration == 0)
                maxDuration = 2;

            cts.CancelAfter(maxDuration * 1000 + 1000);
            try
            {
                await threadPattern.StartPatternAsync(cts.Token);
            }
            catch (TaskCanceledException)
            {
                // all g
            }
            return calls;
        }

        [TestMethod]
        public async Task ShrinkingGraph_CallbacksAreInvoked()
        {
            var gtp = new GraphThreadPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 4,
                Points = new List<Point>()
                {
                   new Point(){ threadLevel = 2, unitsFromStart = 0 },
                   new Point(){ threadLevel = 5, unitsFromStart = 5 },
                   new Point(){ threadLevel = 0, unitsFromStart = 10 }
                }
            };
            var patternTask = InvokeThreadPattern(gtp);
            _semaphore.Release(3);
            var calls = await patternTask;
            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual(2, calls[0]);
            Assert.AreEqual(5, calls[1]);
            Assert.AreEqual(0, calls[2]);
        }

        [TestMethod]
        public async Task ExpandingGraph_CallbacksAreInvoked()
        {
            var gtp = new GraphThreadPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 5,
                Points = new List<Point>()
                {
                   new Point(){ threadLevel = 2, unitsFromStart = 0 },
                   new Point(){ threadLevel = 5, unitsFromStart = 1 },
                   new Point(){ threadLevel = 0, unitsFromStart = 2 }
                }
            };
            var patternTask = InvokeThreadPattern(gtp);
            _semaphore.Release(3);
            var calls = await patternTask;
            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual(2, calls[0]);
            Assert.AreEqual(5, calls[1]);
            Assert.AreEqual(0, calls[2]);
        }

        [TestMethod]
        public async Task Constant_CallbacksAreInvoked()
        {
            var ctp = new ConstantThreadPattern(_mockDatetimeService.Object)
            {
                ThreadCount = 4
            };
            var patternTask = InvokeThreadPattern(ctp);
            _semaphore.Release(2);
            var calls = await patternTask;
            Assert.AreEqual(1, calls.Count);
            Assert.AreEqual(4, calls[0]);
        }

        [TestMethod]
        public async Task TimedConstant_CallbacksAreInvoked()
        {
            var ctp = new ConstantThreadPattern(_mockDatetimeService.Object)
            {
                ThreadCount = 4,
                DurationSeconds = 2
            };
            var patternTask = InvokeThreadPattern(ctp);
            _semaphore.Release(2);
            var calls = await patternTask;
            Assert.AreEqual(2, calls.Count);
            Assert.AreEqual(4, calls[0]);
            Assert.AreEqual(0, calls[1]);
        }

        [TestMethod]
        public async Task Gradual_CallbacksAreInvoked()
        {
            //todo use xunit theories
            var gtp = new GradualThreadPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 6,
                RampDownSeconds = 2,
                RampUpSeconds = 2,
                StepIntervalSeconds = 1,
                ThreadCount = 6
            };
            var patternTask = InvokeThreadPattern(gtp);
            _semaphore.Release(5);
            var calls = await patternTask;
            Assert.AreEqual(5, calls.Count);
            Assert.AreEqual(0, calls[0]);
            Assert.AreEqual(3, calls[1]);
            Assert.AreEqual(6, calls[2]);
            Assert.AreEqual(3, calls[3]);
            Assert.AreEqual(0, calls[4]);
        }

        [TestMethod]
        public async Task GradualComplex_CallbacksAreInvoked()
        {
            var gtp = new GradualThreadPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 10,
                RampDownSeconds = 5,
                RampUpSeconds = 5,
                StepIntervalSeconds = 1,
                ThreadCount = 4
            };
            var patternTask = InvokeThreadPattern(gtp);
            _semaphore.Release(9);
            var calls = await patternTask;
            Assert.AreEqual(9, calls.Count);
            Assert.AreEqual(0, calls[0]);
            Assert.AreEqual(1, calls[1]);
            Assert.AreEqual(2, calls[2]);
            Assert.AreEqual(3, calls[3]);
            Assert.AreEqual(4, calls[4]);
            Assert.AreEqual(3, calls[5]);
            Assert.AreEqual(2, calls[6]);
            Assert.AreEqual(1, calls[7]);
            Assert.AreEqual(0, calls[8]);
        }

        [TestMethod]
        public async Task GradualWithRampUpDownEdgeAndIntervalCollision_CallbacksAreInvoked()
        {
            var gtp = new GradualThreadPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 10,
                RampDownSeconds = 5,
                RampUpSeconds = 5,
                StepIntervalSeconds = 5,
                ThreadCount = 2
            };
            var patternTask = InvokeThreadPattern(gtp);
            _semaphore.Release(3);
            var calls = await patternTask;
            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual(0, calls[0]);
            Assert.AreEqual(2, calls[1]);
            Assert.AreEqual(0, calls[2]);
        }

        [TestMethod]
        public async Task GradualWithNoRampUpDown_CallbacksAreInvoked()
        {
            var gtp = new GradualThreadPattern(_mockDatetimeService.Object)
            {
                DurationSeconds = 2,
                ThreadCount = 2
            };
            var patternTask = InvokeThreadPattern(gtp);
            _semaphore.Release(3);
            var calls = await patternTask;
            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual(0, calls[0]);
            Assert.AreEqual(2, calls[1]);
            Assert.AreEqual(0, calls[2]);
        }
    }
}
