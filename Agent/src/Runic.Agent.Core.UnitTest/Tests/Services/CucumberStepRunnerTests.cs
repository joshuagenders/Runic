using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
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
            var cucumberRunner = new CucumberStepRunnerService(GetType().GetTypeInfo().Assembly);

            var cucumberDocument =
                @"Feature: MyExample
                  Scenario: MyScenario
                   Given I have a given ""method""
                   When I have a when ""wherever""
                   Then I have a then ""whomever""";

            var step = new Step()
            {
                StepName = "TestStep",
                Cucumber = new CucumberInformation()
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
