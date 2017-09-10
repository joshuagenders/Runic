using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Agent.TestUtility;
using Runic.Framework.Models;
using System.Reflection;

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
            var functionFactory = 
                new FunctionFactory(_environment.Get<IPluginManager>(),
                                    _environment.Get<IDataService>(), 
                                    _environment.Get<IEventHandler>());

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
            var fakeFunction = new FakeFunction();
            _environment.GetMock<IPluginManager>()
                        .Setup(p => p.GetInstance("ClassName"))
                        .Returns(fakeFunction);

            _environment.GetMock<IDataService>()
                        .Setup(d => d.GetParams(It.IsAny<string[]>(), It.IsAny<MethodInfo>()))
                        .Returns(new object[] { });

            var function = functionFactory.CreateFunction(step, new Framework.Models.TestContext());
            function.Should().NotBeNull();
        }
    }
}
