﻿using Runic.Configuration;
using Runic.Orchestration;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Runic.UnitTest.Runic
{
    public class TestTimedAction
    {
        [Fact]
        public async void TestTimedActionResponse()
        {
            AppConfiguration.BuildConfiguration(new Dictionary<string,string>()
            {
                { "Database:Host", "http://localhost" },
                { "Database:Prefix", "Runic.Tests." },
                { "Database:Port", "7878" }
            });

            var result = await new TimedAction("someaction", () =>
            {
                Thread.Sleep(1);
                return "result";
            }).Execute();

            Assert.Equal(result.ExecutionResult, "result");
            Assert.True(result.ElapsedMilliseconds > 0);
        }
    }
}