using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Models;
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
            var mockPluginManager = new Mock<IPluginManager>();
            var mockDataService = new Mock<IDataService>();
            var mockEventHandler = new Mock<IEventHandler>();
            var functionFactory = new FunctionFactory(mockPluginManager.Object, mockDataService.Object, mockEventHandler.Object);

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
            mockPluginManager.Setup(p => p.GetInstance("ClassName")).Returns(fakeFunction);
            mockDataService.Setup(d => d.GetParams(It.IsAny<string[]>(), It.IsAny<MethodInfo>())).Returns(new object[] { });
            var function = functionFactory.CreateFunction(step, new Framework.Models.TestContext());
            function.Should().NotBeNull();
        }
    }
}
