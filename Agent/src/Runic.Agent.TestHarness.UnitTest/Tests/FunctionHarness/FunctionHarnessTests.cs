using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.TestHarness.UnitTest.TestUtility;
using Runic.Agent.Framework.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class FunctionHarnessTests
    {
        private TestHarness.Harness.FunctionHarness GetFunctionHarness(object instance = null, Step step = null)
        {
            var fakeFunction = instance ?? new FakeFunction();

            var fakeStep = step ?? new Step()
            {
                StepName = "step1",
                Function = new FunctionInformation()
                {
                    FunctionName = "Login"
                },
                
            };
            return new TestHarness.Harness.FunctionHarness(fakeFunction, fakeStep);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenAFunctionIsExecuted_MethodsAreInvoked()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            var fakeFunction = new FakeFunction();
            var step = new Step() { StepName = "Step1", Function = new FunctionInformation() { FunctionName = "Login" } };
            
            var functionHarness = GetFunctionHarness(fakeFunction, step);
            var result = await functionHarness.ExecuteAsync(cts.Token);

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
            await functionHarness.ExecuteAsync(cts.Token);
            
            Assert.IsTrue(fakeFunction.AsyncTask.IsCompleted);
            AssertFunctionCall(fakeFunction, "AsyncWait");
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenInputParametersAreBound_InputsArePassedToMethods()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            var fakeFunction = new FakeFunction();

            var uniqueString = Guid.NewGuid().ToString("n");
            var randomInt = new Random().Next();

            var step = new Step()
            {
                StepName = "step1",
                Function = new FunctionInformation()
                {
                    FunctionName = "Inputs",
                    PositionalMethodParameterValues = new List<string>(){ uniqueString, randomInt.ToString() }
                }
            };

            var functionHarness = GetFunctionHarness(fakeFunction, step);
            await functionHarness.ExecuteAsync(cts.Token);

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
            var uniqueString = Guid.NewGuid().ToString("n");
            var uniqueString2 = Guid.NewGuid().ToString("n");
            var randomInt = new Random().Next();

            var step = new Step()
            {
                StepName = "step1",
                Function = new FunctionInformation()
                {
                    FunctionName = "InputsWithDefault",
                    PositionalMethodParameterValues = new List<string>(){ uniqueString, randomInt.ToString(), uniqueString2 }
                }
            };

            var functionHarness = GetFunctionHarness(fakeFunction, step);
            await functionHarness.ExecuteAsync(cts.Token);

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
            var uniqueString = Guid.NewGuid().ToString("n");
            var randomInt = new Random().Next();

            var step = new Step()
            {
                StepName = "step1",
                Function = new FunctionInformation()
                {
                    FunctionName = "InputsWithDefault",
                    PositionalMethodParameterValues = new List<string>(){ uniqueString, randomInt.ToString() }
                }
            };

            var functionHarness = GetFunctionHarness(fakeFunction, step);
            await functionHarness.ExecuteAsync(cts.Token);
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
