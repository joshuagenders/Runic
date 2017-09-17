using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Framework.Attributes;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Harness;
using Runic.Agent.TestHarness.Services;
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
            var dataService = new Mock<IDataService>();
            var functionFactory = new FunctionFactory(GetType().GetTypeInfo().Assembly, dataService.Object);

            var step = new Step()
            {
                StepName = "Step1",
                Function = new FunctionInformation()
                {
                    AssemblyQualifiedClassName = GetType().GetTypeInfo().FullName,
                    FunctionName = "Login"
                }
            };

            dataService.Setup(d => d.GetParams(It.IsAny<string[]>(), It.IsAny<MethodInfo>()))
                       .Returns(new object[] { });

            var function = functionFactory.CreateFunction(step, new Framework.Models.TestContext());
            function.Should().NotBeNull();
        }

        [Function("Login")]
        public void Login()
        {
            // Method intentionally left empty.
        }
    }
}
