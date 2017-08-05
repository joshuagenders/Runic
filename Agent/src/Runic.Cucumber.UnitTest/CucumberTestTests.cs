using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Runic.Cucumber.UnitTest.TestUtility;
using System.Collections.Generic;

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
                      .And("I have a method with no inputs")
                      .When("I have a when \"whenstring\"")
                      .Then("I have a then \"thenstring\"")
                      .ExecuteAsync();

            fakeTest.CallList.Count(c => c.InvocationTarget == "GivenMethod" && c.AdditionalData == "givenstring").Should().Be(1);
            fakeTest.CallList.Count(c => c.InvocationTarget == "NoInputs").Should().Be(1);
            fakeTest.CallList.Count(c => c.InvocationTarget == "WhenMethod" && c.AdditionalData == "whenstring").Should().Be(1);
            fakeTest.CallList.Count(c => c.InvocationTarget == "ThenMethod" && c.AdditionalData == "thenstring").Should().Be(1);
        }

        [TestMethod]
        public async Task ExecuteExamples()
        {
            var fakeTest = new FakeCucumberClass();
            TestEnvironment.SetupMocks(fakeTest);
            var test = new CucumberTest(TestEnvironment.AssemblyAdapter.Instance);
            var examples = new Dictionary<string, List<string>>()
                      {
                          { "input1" , new List<string>(){ "givenstring1", "givenstring2" } },
                          { "input2" , new List<string>(){ "whenstring1", "whenstring2" } },
                          { "input3" , new List<string>(){ "thenstring1", "thenstring2" } }
                      };
            var cts = new CancellationTokenSource();
            await test.ScenarioOutline("My Scenario Outline")
                      .Given(@"I have a given ""<input1>""")
                      .And("I have a method with no inputs")
                      .When(@"I have a when ""<input2>""")
                      .Then(@"I have a then ""<input3>""")
                      .Examples(examples)
                      .ExecuteAsync();

            VerifyList(fakeTest.CallList, examples.SelectMany(e => e.Value).ToList());
        }

        public void VerifyList(List<InvocationInformation> callList, List<string> additionalData)
        {
            additionalData.ForEach(d => callList.Count(c => c.AdditionalData == d).Should().Be(1));
        }
    }
}
