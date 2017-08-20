﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Cucumber.UnitTest.TestUtility;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Cucumber.UnitTest.Tests
{
    [TestClass]
    public class DocumentRunnerTests : TestBase
    {
        [TestMethod]
        public async Task WhenAGivenWhenThenDocumentIsExecuted_CallsAssemblyAdapter()
        {
            var fakeTest = new Mock<FakeCucumberClass>();
            TestEnvironment.SetupMocks(fakeTest);
            var assemblyAdapter = new Mock<IAssemblyAdapter>();
            var documentRunner = new DocumentRunner(assemblyAdapter.Object);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            var statement = 
                @"Feature: MyExample
                  Scenario: MyScenario
                   Given I have a given ""method""
                   When I have a when ""wherever""
                   Then I have a then ""whomever""";

            await documentRunner.ExecuteAsync(statement, cts.Token);
            assemblyAdapter.Verify(a => 
                a.ExecuteMethodFromStatementAsync(
                    @"I have a given ""method""", 
                    It.IsAny<object[]>(), 
                    It.IsAny<CancellationToken>()));

            assemblyAdapter.Verify(a =>
                a.ExecuteMethodFromStatementAsync(
                    @"I have a when ""wherever""",
                    It.IsAny<object[]>(),
                    It.IsAny<CancellationToken>()));

            assemblyAdapter.Verify(a =>
                a.ExecuteMethodFromStatementAsync(
                    @"I have a then ""whomever""",
                    It.IsAny<object[]>(),
                    It.IsAny<CancellationToken>()));

        }

        [TestMethod]
        public async Task WhenBadDocumentIsExecuted_ReturnsParserError()
        {
            var assemblyAdapter = new Mock<IAssemblyAdapter>();
            var documentRunner = new DocumentRunner(assemblyAdapter.Object);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            var statement =
                @"Featen I have a given ""method""
                   Whdfdfen I fdfhave a when ""wherever""
                   Thenfdf I have a then ""whomever""";

            var result = await documentRunner.ExecuteAsync(statement, cts.Token);
            Assert.IsTrue(result.Exception.GetType() == typeof(GherkinDocumentParserError));
        }
    }
}