using System.Threading;
using System.Threading.Tasks;
using Runic.Cucumber.UnitTest.TestUtility;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace Runic.Cucumber.UnitTest.Tests
{
    public class CucumberTestTests : TestBase
    {
        [Fact]
        public async Task WhenCucumberTestIsExecuted_ExecutesAllMethodsWithParameters()
        {
            var fakeTest = new Mock<FakeCucumberClass>();
            TestEnvironment.SetupMocks(fakeTest);
            var test = new CucumberTest(TestEnvironment.AssemblyAdapter.Instance);

            var cts = new CancellationTokenSource();
            await test.Given("I have a given \"givenstring\"")
                      .And("I have a method with no inputs")
                      .When("I have a when \"whenstring\"")
                      .Then("I have a then \"thenstring\"")
                      .ExecuteAsync(cts.Token);

            fakeTest.Verify(f => f.GivenMethod("givenstring"));
            fakeTest.Verify(f => f.WhenMethod("whenstring"));
            fakeTest.Verify(f => f.ThenMethod("thenstring"));
            fakeTest.Verify(f => f.NoInputs());
        }

        [Fact]
        public async Task WhenCucumberTestWithExampleExecuted_InvokesMultipleTimesWithParameters()
        {
            var fakeTest = new Mock<FakeCucumberClass>();
            TestEnvironment.SetupMocks(fakeTest);
            var test = new CucumberTest(TestEnvironment.AssemblyAdapter.Instance);
            var examples = new Dictionary<string, List<string>>()
                      {
                          { "given" , new List<string>(){ "givenstring1", "givenstring2" } },
                          { "when" , new List<string>(){ "whenstring1", "whenstring2" } },
                          { "inthenput3" , new List<string>(){ "thenstring1", "thenstring2" } }
                      };
            var cts = new CancellationTokenSource();
            await test.ScenarioOutline("My Scenario Outline")
                      .Given(@"I have a given ""<given>""")
                      .And("I have a method with no inputs")
                      .When(@"I have a when ""<when>""")
                      .Then(@"I have a then ""<then>""")
                      .Examples(examples)
                      .ExecuteAsync(cts.Token);

            VerifyList(fakeTest, examples);
        }

        private void VerifyList(Mock<FakeCucumberClass> fakeTest, Dictionary<string, List<string>> examples)
        {
            foreach (var example in examples)
            {
                switch (example.Key)
                {
                    case "given":
                        example.Value.ForEach(e => fakeTest.Verify(f => f.GivenMethod(e)));
                        break;
                    case "when":
                        example.Value.ForEach(e => fakeTest.Verify(f => f.WhenMethod(e)));
                        break;
                    case "then":
                        example.Value.ForEach(e => fakeTest.Verify(f => f.ThenMethod(e)));
                        break;
                }
            }
        }
    }
}
