using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Configuration;
using Runic.Framework.Orchestration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Runic.UnitTest.Runic
{
    [TestClass]
    public class TestTimedAction
    {
        [TestMethod]
        public async Task TestTimedActionResponse()
        {
            RunicConfiguration.BuildConfiguration(new Dictionary<string, string>
            {
                {"Database:Host", "http://localhost"},
                {"Database:Prefix", "Runic.Tests."},
                {"Database:Port", "7878"}
            });

            Func <string> action = () =>
            {
                
                Thread.Sleep(1);
                return "result";
            };
            var result = await action.TimedExecute("sleep_one_millisecond");

            Assert.AreEqual(result.ExecutionResult, "result");
            Assert.IsTrue(result.ElapsedMilliseconds > 0);
        }
    }
}