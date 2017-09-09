using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.CucumberHarness
{
    [TestClass]
    public class CucumberHarnessTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenBasicCucumberTestExecutes_AllMethodsAreInvoked()
        {
            const string assemblyName = "thisassembly";
            var pluginManager = new Mock<IPluginManager>();
            pluginManager.Setup(p => p.GetPlugin(assemblyName)).Returns(GetType().GetTypeInfo().Assembly);
            var harness = new Core.CucumberHarness.CucumberHarness(pluginManager.Object);

            var cucumberDocument = 
                @"Feature: MyExample
                  Scenario: MyScenario
                   Given I have a given ""method""
                   When I have a when ""wherever""
                   Then I have a then ""whomever""";

            var cts = new CancellationTokenSource();
            await harness.ExecuteTestAsync(assemblyName, cucumberDocument, cts.Token);
            pluginManager.Verify(p => p.GetPlugin(assemblyName));

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
