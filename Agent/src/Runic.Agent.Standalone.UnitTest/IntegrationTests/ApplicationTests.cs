using Runic.Cucumber;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Runic.Agent.Standalone.Test.IntegrationTests
{
    public class ApplicationTests
    {
        [Theory]
        [InlineData("Constant")]
        [InlineData("Graph")]
        [InlineData("Gradual")]
        public async Task TestPatternExecution(string patternType)
        {
            var test = new CucumberTest(GetType().GetTypeInfo().Assembly);
            test.Given($"I have a test environment for a '{patternType}' flow")
                .And("I have a function flow")
                .And("I start the application")
                .When("I start the test")
                .Then("The fake cucumber test is invoked");
            
            var result = await test.ExecuteAsync();
            if (!result.Success)
            {
                throw result.Exception;
            }
        }

        [Fact]
        public async Task TestCucumberExecution()
        {
            var test = new CucumberTest(GetType().GetTypeInfo().Assembly);
            test.ScenarioOutline("Cucumber patterns")
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

            var result = await test.ExecuteAsync();
            if (!result.Success)
            {
                throw result.Exception;
            }
        }
    }
}
