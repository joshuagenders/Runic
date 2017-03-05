using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Configuration;
using Runic.Framework.Orchestration;
using NUnit.Framework;

namespace Runic.UnitTest.Runic
{
    public class TestTimedAction
    {
        [Test]
        public async Task TestTimedActionResponse()
        {
            RunicConfiguration.BuildConfiguration(new Dictionary<string, string>
            {
                {"Database:Host", "http://localhost"},
                {"Database:Prefix", "Runic.Tests."},
                {"Database:Port", "7878"}
            });

            var result = await new TimedAction("someaction", () =>
            {
                Thread.Sleep(1);
                return "result";
            }).Execute();

            Assert.AreEqual(result.ExecutionResult, "result");
            Assert.IsTrue(result.ElapsedMilliseconds > 0);
        }
    }
}