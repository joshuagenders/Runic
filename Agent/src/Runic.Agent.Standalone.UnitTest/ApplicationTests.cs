﻿using FluentAssertions;
using Runic.Cucumber;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Runic.Agent.Standalone.Test
{
    public class ApplicationTests
    {
        private CucumberTest _test { get; set; }

        public ApplicationTests()
        {
            _test = new CucumberTest(GetType().GetTypeInfo().Assembly);
        }

        [Theory]
        [InlineData("Constant")]
        [InlineData("Graph")]
        [InlineData("Gradual")]
        public async Task TestPatternExecution(string patternType)
        {
            _test.Given($"I have a test environment for a '{patternType}' flow")
                 .And("I have a function flow")
                 .And("I start the application")
                 .When("I start the test")
                 .Then("The fake function is invoked");

            var success = await _test.ExecuteAsync();
            success.Should().BeTrue();
        }

        [Fact]
        public async Task TestCucumberExecution()
        {
            _test.ScenarioOutline("Cucumber patterns")
                 .Given($"I have a test environment for a '<patternType>' flow")
                 .And("I have a cucumber flow")
                 .And("I start the application")
                 .When("I start the test")
                 .Then("The fake cucumber test is invoked")
                 .Examples(
                    new Dictionary<string, List<string>>()
                    {
                        { "patternType", new List<string>(){ "Constant", "Graph", "Gradual" } }
                    });

            var success = await _test.ExecuteAsync();
            success.Should().BeTrue();
        }
    }
}
