using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Cucumber;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone.Test.FunctionalTests
{
    [TestClass]
    public class CucumberTests
    {
        [TestCategory("FunctionalTest")]
        [TestMethod]
        public async Task WhenFunctionIsExecuted_ThenTestResultIsSuccess()
        {
            var test = new CucumberTest(GetType().GetTypeInfo().Assembly);
            test.ScenarioOutline("Function Patterns")
                .Given($"I have a test environment for a '<patternType>' flow")
                .And("I have a function flow")
                .And("I start the application")
                .When("I start the test")
                .Then("The fake function is invoked")
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

        [TestCategory("FunctionalTest")]
        [TestMethod]
        public async Task WhenCucumberIsExecuted_ThenTestResultIsSuccess()
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
