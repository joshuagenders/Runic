using Runic.Agent.UnitTest.TestUtility;
using Runic.Agent.Core.Harness;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Runic.Agent.UnitTest.Tests
{
    public class CucumberHarnessTests
    {
        [Fact]
        public void WhenBasicCucumberTestExecutes_AllMethodsAreInvoked()
        {
            var harness = new CucumberHarness();

            var cucumberDocument = 
                @"Feature: MyExample
                  Scenario: MyScenario
                   Given I have a given ""method""
                   When I have a when ""wherever""
                   Then I have a then ""whomever""";

            harness.ExecuteTest(GetType().GetTypeInfo().Assembly, cucumberDocument);
            
            var maxKey = FakeCucumberClass.FakeCucumberClasses.Keys.OrderBy(a => a).Last();
            if (!FakeCucumberClass.FakeCucumberClasses.TryGetValue(maxKey, out FakeCucumberClass test))
            {
                Assert.False(true, "No cucumber test class found in static FakeCucumberClasses Dictionary");
            }
            Assert.True(test.CallList.Any(c => c.InvocationTarget == "GivenMethod"), "Given method not called");
            Assert.True(test.CallList.Any(c => c.InvocationTarget == "WhenMethod"), "When method not called");
            Assert.True(test.CallList.Any(c => c.InvocationTarget == "ThenMethod"), "Then method not called");
        }
    }
}
