using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;

namespace Runic.Cucumber.UnitTest
{
    [TestClass]
    public class CucumberTestTests : TestBase
    {
        [TestMethod]
        public async Task CucumberTestExecutesAllMethods()
        {
            var fakeTest = new FakeCucumberClass();
            TestEnvironment.SetupMocks(fakeTest);
            var test = new CucumberTest(TestEnvironment.AssemblyAdapter.Instance);

            var cts = new CancellationTokenSource();
            await test.Given("I have a given \"givenstring\"")
                      .When("I have a when \"whenstring\"")
                      .Then("I have a then \"thenstring\"")
                      .ExecuteAsync();

            fakeTest.CallList.Count(c => c.InvocationTarget == "GivenMethod" && c.AdditionalData == "givenstring").Should().Be(1);
            fakeTest.CallList.Count(c => c.InvocationTarget == "WhenMethod" && c.AdditionalData == "whenstring").Should().Be(1);
            fakeTest.CallList.Count(c => c.InvocationTarget == "ThenMethod" && c.AdditionalData == "thenstring").Should().Be(1);
        }
    }
}
