using Runic.Agent.UnitTest.TestUtility;
using Runic.Agent.Core.Harness;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Runic.Agent.Core.Models;
using FluentAssertions;

namespace Runic.Agent.UnitTest.Tests
{
    public class CucumberHarnessTests
    {
        [Fact]
        public async Task WhenBasicCucumberTestExecutes_AllMethodsAreInvoked()
        {
            var harness = new CucumberHarness();

            var cucumberDocument = 
                @"Feature: MyExample
                  Scenario: MyScenario
                   Given I have a given ""method""
                   When I have a when ""wherever""
                   Then I have a then ""whomever""";

            var result = 
                await harness.ExecuteTestAsync(
                    GetType().GetTypeInfo().Assembly, 
                    new Step("step 1", null, new CucumberStepInformation(cucumberDocument, GetType().GetTypeInfo().Assembly.FullName)));
            result.Success.Should().BeTrue();
        }
    }
}
