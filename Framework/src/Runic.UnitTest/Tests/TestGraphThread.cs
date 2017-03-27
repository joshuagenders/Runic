using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Framework.ThreadPatterns;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.UnitTest.Tests
{
    [TestClass]
    public class TestGraphThread
    {
        [TestMethod]
        public async Task TestCallbacks()
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
    }
}
