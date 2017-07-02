using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Framework.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestThreadPatterns
    {
        public async Task <List<int>> InvokeThreadPattern(IThreadPattern threadPattern)
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
        public async Task Patterns_ShrinkingGraphCallbacks()
        {
            var gtp = new GraphThreadPattern()
            {
                DurationSeconds = 4,
                Points = new List<Point>()
                {
                   new Point(){ threadLevel = 2, unitsFromStart = 0 },
                   new Point(){ threadLevel = 5, unitsFromStart = 5 },
                   new Point(){ threadLevel = 0, unitsFromStart = 10 }
                }
            };
            var calls = await InvokeThreadPattern(gtp);
            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual(2, calls[0]);
            Assert.AreEqual(5, calls[1]);
            Assert.AreEqual(0, calls[2]);
        }

        [TestMethod]
        public async Task Patterns_ExpandingGraphCallbacks()
        {
            var gtp = new GraphThreadPattern()
            {
                DurationSeconds = 5,
                Points = new List<Point>()
                {
                   new Point(){ threadLevel = 2, unitsFromStart = 0 },
                   new Point(){ threadLevel = 5, unitsFromStart = 1 },
                   new Point(){ threadLevel = 0, unitsFromStart = 2 }
                }
            };
            var calls = await InvokeThreadPattern(gtp);
            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual(2, calls[0]);
            Assert.AreEqual(5, calls[1]);
            Assert.AreEqual(0, calls[2]);
        }

        [TestMethod]
        public async Task Patterns_ConstantCallback()
        {
            var ctp = new ConstantThreadPattern()
            {
                ThreadCount = 4
            };
            var calls = await InvokeThreadPattern(ctp);
            Assert.AreEqual(1, calls.Count);
            Assert.AreEqual(4, calls[0]);
        }

        [TestMethod]
        public async Task Patterns_TimedConstantCallback()
        {
            var ctp = new ConstantThreadPattern()
            {
                ThreadCount = 4,
                DurationSeconds = 2
            };
            var calls = await InvokeThreadPattern(ctp);
            Assert.AreEqual(2, calls.Count);
            Assert.AreEqual(4, calls[0]);
            Assert.AreEqual(0, calls[1]);
        }

        [TestMethod]
        public async Task Patterns_GradualCallback()
        {
            //todo use xunit theories
            var gtp = new GradualThreadPattern()
            {
                DurationSeconds = 6,
                RampDownSeconds = 2,
                RampUpSeconds = 2,
                StepIntervalSeconds = 1,
                ThreadCount = 6
            };
            var calls = await InvokeThreadPattern(gtp);
            Assert.AreEqual(5, calls.Count);
            Assert.AreEqual(0, calls[0]);
            Assert.AreEqual(3, calls[1]);
            Assert.AreEqual(6, calls[2]);
            Assert.AreEqual(3, calls[3]);
            Assert.AreEqual(0, calls[4]);
        }

        [TestMethod]
        public async Task Patterns_GradualCallbackComplex()
        {
            var gtp = new GradualThreadPattern()
            {
                DurationSeconds = 10,
                RampDownSeconds = 5,
                RampUpSeconds = 5,
                StepIntervalSeconds = 1,
                ThreadCount = 4
            };
            var calls = await InvokeThreadPattern(gtp);
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
        public async Task Patterns_GradualCallbackRampUpDownEdgeAndIntervalCollision()
        {
            var gtp = new GradualThreadPattern()
            {
                DurationSeconds = 10,
                RampDownSeconds = 5,
                RampUpSeconds = 5,
                StepIntervalSeconds = 5,
                ThreadCount = 2
            };
            var calls = await InvokeThreadPattern(gtp);
            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual(0, calls[0]);
            Assert.AreEqual(2, calls[1]);
            Assert.AreEqual(0, calls[2]);
        }

        [TestMethod]
        public async Task Patterns_GradualCallbackNoRampUpDown()
        {
            var gtp = new GradualThreadPattern()
            {
                DurationSeconds = 2,
                ThreadCount = 2
            };
            var calls = await InvokeThreadPattern(gtp);
            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual(0, calls[0]);
            Assert.AreEqual(2, calls[1]);
            Assert.AreEqual(0, calls[2]);
        }
    }
}
