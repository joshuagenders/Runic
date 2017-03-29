using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.ThreadPatterns;
using Runic.Framework.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestThreadPatterns
    {
        [TestMethod]
        public async Task TestShrinkingGraphCallbacks()
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
            var calls = new List<int>();
            gtp.RegisterThreadChangeHandler((threadCount) =>
            {
                calls.Add(threadCount);
            });

            var cts = new CancellationTokenSource();
            cts.CancelAfter(4500);
            await gtp.Start(cts.Token);
            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual(2, calls[0]);
            Assert.AreEqual(5, calls[1]);
            Assert.AreEqual(0, calls[2]);
        }

        [TestMethod]
        public async Task TestExpandingGraphCallbacks()
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
            var calls = new List<int>();
            gtp.RegisterThreadChangeHandler((threadCount) =>
            {
                calls.Add(threadCount);
            });

            var cts = new CancellationTokenSource();
            cts.CancelAfter(5500);
            await gtp.Start(cts.Token);
            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual(2, calls[0]);
            Assert.AreEqual(5, calls[1]);
            Assert.AreEqual(0, calls[2]);
        }

        [TestMethod]
        public async Task TestConstantCallback()
        {
            var gtp = new ConstantThreadPattern()
            {
                ThreadCount = 4
            };
            var calls = new List<int>();
            gtp.RegisterThreadChangeHandler((threadCount) =>
            {
                calls.Add(threadCount);
            });

            var cts = new CancellationTokenSource();
            cts.CancelAfter(200);
            await gtp.Start(cts.Token);
            Assert.AreEqual(1, calls.Count);
            Assert.AreEqual(4, calls[0]);
        }

        [TestMethod]
        public async Task TestTimedConstantCallback()
        {
            var gtp = new ConstantThreadPattern()
            {
                ThreadCount = 4,
                DurationSeconds = 2
            };
            var calls = new List<int>();
            gtp.RegisterThreadChangeHandler((threadCount) =>
            {
                calls.Add(threadCount);
            });

            var cts = new CancellationTokenSource();
            cts.CancelAfter(2200);
            await gtp.Start(cts.Token);
            Assert.AreEqual(2, calls.Count);
            Assert.AreEqual(4, calls[0]);
            Assert.AreEqual(0, calls[1]);
        }

        /*
         todo test edge cases
         var gtp = new GradualThreadPattern()
            {
                DurationSeconds = 10,
                RampDownSeconds = 5,
                RampUpSeconds = 5,
                ThreadCount = 2
            };
             */

        [TestMethod]
        public async Task TestGradualCallback()
        {
            var gtp = new GradualThreadPattern()
            {
                DurationSeconds = 6,
                RampDownSeconds = 2,
                RampUpSeconds = 2,
                ThreadCount = 6,
                StepIntervalSeconds = 1
            };
            var calls = new List<int>();
            gtp.RegisterThreadChangeHandler((threadCount) =>
            {
                calls.Add(threadCount);
            });

            var cts = new CancellationTokenSource();
            cts.CancelAfter(6500);
            await gtp.Start(cts.Token);
            Assert.AreEqual(5, calls.Count);
            Assert.AreEqual(0, calls[0]);
            Assert.AreEqual(3, calls[1]);
            Assert.AreEqual(6, calls[2]);
            Assert.AreEqual(3, calls[3]);
            Assert.AreEqual(0, calls[4]);
        }
    }
}
