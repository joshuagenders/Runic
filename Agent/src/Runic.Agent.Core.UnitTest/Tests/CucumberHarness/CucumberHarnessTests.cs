using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.UnitTest.TestUtility;
using Runic.Agent.Harness;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class CucumberHarnessTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenBasicCucumberTestExecutes_AllMethodsAreInvoked()
        {
            var harness = new CucumberHarness();

            var cucumberDocument = 
                @"Feature: MyExample
                  Scenario: MyScenario
                   Given I have a given ""method""
                   When I have a when ""wherever""
                   Then I have a then ""whomever""";

            var cts = new CancellationTokenSource();
            await harness.ExecuteTestAsync(GetType().GetTypeInfo().Assembly, cucumberDocument, cts.Token);
            
            var maxKey = FakeCucumberClass.FakeCucumberClasses.Keys.OrderBy(a => a).Last();
            if (!FakeCucumberClass.FakeCucumberClasses.TryGetValue(maxKey, out FakeCucumberClass test))
            {
                Assert.Fail("No cucumber test class found in static FakeCucumberClasses Dictionary");
            }
            Assert.IsTrue(test.CallList.Any(c => c.InvocationTarget == "GivenMethod"), "Given method not called");
            Assert.IsTrue(test.CallList.Any(c => c.InvocationTarget == "WhenMethod"), "When method not called");
            Assert.IsTrue(test.CallList.Any(c => c.InvocationTarget == "ThenMethod"), "Then method not called");
        }
    }
}
