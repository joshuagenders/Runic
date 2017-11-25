using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.UnitTest.TestUtility;
using Runic.Agent.Core.Models;
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
        private Harness.FunctionHarness GetFunctionHarness(object instance = null, Step step = null)
        {
            var fakeFunction = instance ?? new FakeFunction();

            var fakeStep = step ?? new Step()
            {
                StepName = "step1",
                Function = new MethodInformation()
                {
                    MethodName = "Login"
                },
                
            };
            return new Harness.FunctionHarness(fakeFunction, fakeStep);
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenAFunctionIsExecuted_MethodsAreInvoked()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            var fakeFunction = new FakeFunction();
            var step = new Step() { StepName = "Step1", Function = new MethodInformation() { MethodName = "Login" } };
            
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
            var step = new Step() { Function = new MethodInformation() { MethodName = "AsyncWait" } };
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
                Function = new MethodInformation()
                {
                    MethodName = "Inputs",
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
                Function = new MethodInformation()
                {
                    MethodName = "InputsWithDefault",
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
                Function = new MethodInformation()
                {
                    MethodName = "InputsWithDefault",
                    PositionalMethodParameterValues = new List<string>(){ uniqueString, randomInt.ToString() }
                }
            };

            var functionHarness = GetFunctionHarness(fakeFunction, step);
            await functionHarness.ExecuteAsync(cts.Token);
            AssertFunctionCall(fakeFunction, "InputsWithDefault");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.AdditionalData == $"input1={uniqueString},input2={randomInt},input3=default"));
        }

        private void AssertFunctionCall(FakeFunction fakeFunction, string MethodName)
        {
            Assert.AreEqual(1, fakeFunction.CallList.Count);
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == MethodName), $"{MethodName} called");
        }
    }
}
