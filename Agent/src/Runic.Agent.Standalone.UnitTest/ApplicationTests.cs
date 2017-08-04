using FluentAssertions;
using Runic.Cucumber;
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
                 .When("I start the test")
                 .Then("The fake function is invoked");

            var success = await _test.ExecuteAsync();
            success.Should().BeTrue();
        }
    }
}
