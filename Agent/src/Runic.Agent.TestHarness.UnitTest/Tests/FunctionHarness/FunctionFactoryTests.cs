using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.TestHarness.Harness;
using Runic.Agent.TestHarness.UnitTest.TestUtility;
using Runic.Agent.TestUtility;
using Runic.Agent.Framework.Models;
using System.Reflection;
using Runic.Agent.TestHarness.Services;

namespace Runic.Agent.Core.UnitTest.Tests.FunctionHarness
{
    [TestClass]
    public class FunctionFactoryTests
    {
        private TestEnvironmentBuilder _environment { get; set; }

        [TestInitialize]
        public void Init()
        {
            _environment = new UnitEnvironment();
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenCreatingFunction_ThenFunctionIsReturned()
        {
            var functionFactory = new FunctionFactory(GetType().GetTypeInfo().Assembly, _environment.Get<IDataService>());

            var step = new Step()
            {
                StepName = "Step1",
                Function = new FunctionInformation()
                {
                    AssemblyQualifiedClassName = "ClassName",
                    FunctionName = "Login",
                    AssemblyName = "AssemblyName"
                }
            };

            _environment.GetMock<IDataService>()
                        .Setup(d => d.GetParams(It.IsAny<string[]>(), It.IsAny<MethodInfo>()))
                        .Returns(new object[] { });

            var function = functionFactory.CreateFunction(step, new Framework.Models.TestContext());
            function.Should().NotBeNull();
        }
    }
}
