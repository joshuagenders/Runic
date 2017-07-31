using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Cucumber.UnitTest
{
    [TestClass]
    public class DocumentRunnerTests : TestBase
    {
        [TestMethod]
        public async Task GivenWhenThenDocumentCallsAllMethods()
        {
            var fakeTest = new FakeBddTest();
            TestEnvironment.SetupMocks(fakeTest);
            var method = fakeTest.GetType().GetTypeInfo().GetMethod("GivenMethod");
            var assemblyAdapter = new AssemblyAdapter(fakeTest.GetType().GetTypeInfo().Assembly, TestEnvironment.TestStateManager.Instance);
            var documentRunner = new DocumentRunner(assemblyAdapter);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            var statement = 
                @"Feature: MyExample
                  Scenario: MyScenario
                   Given I have a given ""method""
                   When I have a when ""wherever""
                   Then I have a then ""whomever""";

            await documentRunner.ExecuteAsync(statement, cts.Token);

            fakeTest.CallList.Count(c => c.InvocationTarget == "GivenMethod").Should().Be(1);
            fakeTest.CallList.Count(c => c.InvocationTarget == "WhenMethod").Should().Be(1);
            fakeTest.CallList.Count(c => c.InvocationTarget == "ThenMethod").Should().Be(1);
        }
    }
}
