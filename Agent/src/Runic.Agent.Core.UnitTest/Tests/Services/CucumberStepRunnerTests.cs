using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Models;
using System.Reflection;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.Services
{
    [TestClass]
    public class CucumberStepRunnerTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenCucumberStepRunnerIsExecuted_MethodsAreInvoked()
        {
            var environment = new TestEnvironment();

            environment.PluginManager
                       .Setup(p => p.GetPlugin(It.IsAny<string>()))
                       .Returns(GetType().GetTypeInfo().Assembly);
            var cucumberRunner = new CucumberStepRunnerService(environment.PluginManager.Object);

            var cucumberDocument =
                @"Feature: MyExample
                  Scenario: MyScenario
                   Given I have a given ""method""
                   When I have a when ""wherever""
                   Then I have a then ""whomever""";

            var step = new Step()
            {
                StepName = "TestStep",
                Cucumber = new CucmberInformation()
                {
                    AssemblyName = "thisassembly",
                    Document = cucumberDocument
                }
            };

            var result = await cucumberRunner.ExecuteStepAsync(step);
            result.Success.Should().BeTrue();
            result.NextStep.Should().BeNullOrEmpty();
        }
    }
}
