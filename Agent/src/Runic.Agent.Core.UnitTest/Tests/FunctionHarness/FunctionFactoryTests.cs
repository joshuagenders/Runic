using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Harness;
using System.Reflection;

namespace Runic.Agent.Core.UnitTest.Tests.FunctionHarness
{
    [TestClass]
    public class FunctionFactoryTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenCreatingFunction_ThenFunctionIsReturned()
        {
            var functionFactory = new FunctionFactory(GetType().GetTypeInfo().Assembly);

            var step = new Step()
            {
                StepName = "Step1",
                Function = new MethodInformation()
                {
                    AssemblyQualifiedClassName = GetType().GetTypeInfo().FullName,
                    MethodName = "Login"
                }
            };

            var function = functionFactory.CreateFunction(step, new Models.TestContext());
            function.Should().NotBeNull();
        }

        public void Login()
        {
            // Method intentionally left empty.
        }
    }
}
