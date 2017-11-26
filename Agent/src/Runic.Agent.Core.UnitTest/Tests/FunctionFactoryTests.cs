using FluentAssertions;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Harness;
using System.Reflection;
using Xunit;

namespace Runic.Agent.Core.UnitTest.Tests
{
    public class FunctionFactoryTests
    {
        [Fact]
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

            var function = functionFactory.CreateFunction(step, new TestContext());
            function.Should().NotBeNull();
        }

        protected void Login()
        {
            // Method intentionally left empty.
        }
    }
}
