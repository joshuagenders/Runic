using Moq;
using Runic.Cucumber.UnitTest.TestUtility;
using System.Threading;
using Xunit;

namespace Runic.Cucumber.UnitTest.Tests
{
    public class DocumentRunnerTests : TestBase
    {
        [Fact]
        public void WhenAGivenWhenThenDocumentIsExecuted_CallsAssemblyAdapter()
        {
            var fakeTest = new Mock<FakeCucumberClass>();
            TestEnvironment.SetupMocks(fakeTest);
            var assemblyAdapter = new Mock<IAssemblyAdapter>();
            var documentRunner = new DocumentRunner(assemblyAdapter.Object);

            var statement = 
                @"Feature: MyExample
                  Scenario: MyScenario
                   Given I have a given ""method""
                   When I have a when ""wherever""
                   Then I have a then ""whomever""";

            documentRunner.Execute(statement);
            assemblyAdapter.Verify(a => 
                a.ExecuteMethodFromStatement(
                    @"I have a given ""method""", 
                    It.IsAny<object[]>()));

            assemblyAdapter.Verify(a =>
                a.ExecuteMethodFromStatement(
                    @"I have a when ""wherever""",
                    It.IsAny<object[]>()));

            assemblyAdapter.Verify(a =>
                a.ExecuteMethodFromStatement(
                    @"I have a then ""whomever""",
                    It.IsAny<object[]>()));
        }
        
        [Fact]
        public void WhenBadDocumentIsExecuted_ReturnsParserError()
        {
            var assemblyAdapter = new Mock<IAssemblyAdapter>();
            var documentRunner = new DocumentRunner(assemblyAdapter.Object);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            var statement =
                @"Featen I have a given ""method""
                   Whdfdfen I fdfhave a when ""wherever""
                   Thenfdf I have a then ""whomever""";

            var result = documentRunner.Execute(statement);
            Assert.True(result.Exception.GetType() == typeof(GherkinDocumentParserError));
        }
    }
}
