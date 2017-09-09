using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Agent.Core.Services;
using Runic.Framework.Attributes;
using Runic.Framework.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Runic.Agent.Core.UnitTest.Tests.t
{
    [TestClass]
    public class FunctionHarnessTests
    {
        private Core.FunctionHarness.FunctionHarness GetFunctionHarness(object instance = null, Step step = null, IDataService dataService = null)
        {
            var fakeFunction = instance ?? new FakeFunction();
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(d => d.GetParams(It.IsAny<string[]>(), It.IsAny<MethodInfo>())).Returns(new string[] { });
            var functionHarness = new Core.FunctionHarness.FunctionHarness(new Mock<IEventService>().Object, dataService ?? mockDataService.Object);
            var fakeStep = step ?? new Step()
            {
                StepName = "step1",
                Function = new FunctionInformation()
                {
                    FunctionName = "Login"
                },
                
            };
            functionHarness.Bind(fakeFunction, fakeStep);

            return functionHarness;
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenGettingMethodWithAttribute_MethodIsFound()
        {
            var functionHarness = GetFunctionHarness();
            var method = functionHarness.GetMethodWithAttribute(typeof(BeforeEachAttribute));
            Assert.IsNotNull(method, "beforeeach method not found");
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenAFunctionIsBoundAndExecuted_MethodsAreInvoked()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(60000);

            var fakeFunction = new FakeFunction();
            var step = new Step() { StepName = "Step1", Function = new FunctionInformation() { FunctionName = "Login" } };
            
            var functionHarness = GetFunctionHarness(fakeFunction, step);

            var result = await functionHarness.OrchestrateFunctionExecutionAsync(cts.Token);

            Assert.IsTrue(result.Success, "Function returned false - error in execution");
            AssertFunctionCall(fakeFunction, "Login");
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenAsyncFunctionIsCalled_HarnessWaitsToCompletion()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);

            var fakeFunction = new FakeFunction();
            var step = new Step() { Function = new FunctionInformation() { FunctionName = "AsyncWait" } };
            var functionHarness = GetFunctionHarness(fakeFunction, step);

            await functionHarness.OrchestrateFunctionExecutionAsync(cts.Token);
            
            Assert.IsTrue(fakeFunction.AsyncTask.IsCompleted);
            AssertFunctionCall(fakeFunction, "AsyncWait");
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenInputParametersAreBound_InputsArePassedToMethods()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(120000);

            var fakeFunction = new FakeFunction();

            var mockDataService = new Mock<IDataService>();
            var uniqueString = Guid.NewGuid().ToString("n");
            var randomInt = new Random().Next();

            mockDataService.Setup(d => d.GetParams(It.IsAny<string[]>(), It.IsAny<MethodInfo>())).Returns(new object[] { uniqueString, randomInt });

            var step = new Step()
            {
                StepName = "step1",
                Function = new FunctionInformation()
                {
                    FunctionName = "Inputs",
                    Parameters = new List<string>(){ uniqueString, randomInt.ToString() }
                }
            };

            var functionHarness = GetFunctionHarness(fakeFunction, step, mockDataService.Object);
            await functionHarness.OrchestrateFunctionExecutionAsync(cts.Token);

            AssertFunctionCall(fakeFunction, "Inputs");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.AdditionalData == $"input1={uniqueString},input2={randomInt}"));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenInputParametersAreBound_OverrideDefaultOveridesTheInput()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            var fakeFunction = new FakeFunction();
            var mockDataService = new Mock<IDataService>();

            var uniqueString = Guid.NewGuid().ToString("n");
            var uniqueString2 = Guid.NewGuid().ToString("n");
            var randomInt = new Random().Next();

            mockDataService.Setup(d => d.GetParams(It.IsAny<string[]>(), It.IsAny<MethodInfo>())).Returns(new object[] { uniqueString, randomInt, uniqueString2 });

            var step = new Step()
            {
                StepName = "step1",
                Function = new FunctionInformation()
                {
                    FunctionName = "InputsWithDefault",
                    Parameters = new List<string>(){ uniqueString, randomInt.ToString(), uniqueString2 }
                }
            };

            var functionHarness = GetFunctionHarness(fakeFunction, step, mockDataService.Object);
            await functionHarness.OrchestrateFunctionExecutionAsync(cts.Token);

            AssertFunctionCall(fakeFunction, "InputsWithDefault");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.AdditionalData == $"input1={uniqueString},input2={randomInt},input3={uniqueString2}"));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenInvokingMethodWithDefaults_MethodsAreInvokedWithDefault()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            var fakeFunction = new FakeFunction();

            var mockDataService = new Mock<IDataService>();
            var uniqueString = Guid.NewGuid().ToString("n");
            var randomInt = new Random().Next();
            mockDataService.Setup(d => d.GetParams(It.IsAny<string[]>(), It.IsAny<MethodInfo>())).Returns(new object[]{ uniqueString, randomInt, "default" });
          
            var step = new Step()
            {
                StepName = "step1",
                Function = new FunctionInformation()
                {
                    FunctionName = "InputsWithDefault",
                    Parameters = new List<string>(){ uniqueString, randomInt.ToString() }
                }
            };

            var functionHarness = GetFunctionHarness(fakeFunction, step, mockDataService.Object);
            await functionHarness.OrchestrateFunctionExecutionAsync(cts.Token);
            AssertFunctionCall(fakeFunction, "InputsWithDefault");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.AdditionalData == $"input1={uniqueString},input2={randomInt},input3=default"));
        }

        private void AssertFunctionCall(FakeFunction fakeFunction, string functionName)
        {
            Assert.AreEqual(3, fakeFunction.CallList.Count);
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "BeforeEach"), "BeforeEach called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == functionName), $"{functionName} called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "AfterEach"), "AfterEach called");
        }
    }
}
