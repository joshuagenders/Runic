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
            var fakeTest = new FakeBddTest();
            TestEnvironment.SetupMocks(fakeTest);
            var test = new CucumberTest(TestEnvironment.AssemblyAdapter.Instance);

            var cts = new CancellationTokenSource();
            await test.Given("I have a given \"string\"")
                      .When("I have a when \"string\"")
                      .Then("I have a then \"string\"")
                      .ExecuteAsync();

            fakeTest.CallList.Count(c => c.InvocationTarget == "GivenMethod").Should().Be(1);
            fakeTest.CallList.Count(c => c.InvocationTarget == "WhenMethod").Should().Be(1);
            fakeTest.CallList.Count(c => c.InvocationTarget == "ThenMethod").Should().Be(1);
        }
    }
}
