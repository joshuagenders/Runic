using Runic.Agent.UnitTest.TestUtility;
using Runic.Agent.Core.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Runic.Agent.Core.Harness;
using Xunit;
using FluentAssertions;

namespace Runic.Agent.Core.UnitTest.Tests
{
    public class FunctionHarnessTests
    {
        private FunctionHarness GetFunctionHarness(object instance = null, Step step = null)
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
            return new FunctionHarness(fakeFunction, fakeStep);
        }
        
        [Fact]
        public async Task WhenAFunctionIsExecuted_MethodsAreInvoked()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            var fakeFunction = new FakeFunction();
            var step = new Step() { StepName = "Step1", Function = new MethodInformation() { MethodName = "Login" } };
            
            var functionHarness = GetFunctionHarness(fakeFunction, step);
            var result = await functionHarness.ExecuteAsync(cts.Token);

            result.Success.Should().BeTrue("Function returned false - error in execution");
            AssertFunctionCall(fakeFunction, "Login");
        }
        
        [Fact]
        public async Task WhenAsyncFunctionIsCalled_HarnessWaitsToCompletion()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);

            var fakeFunction = new FakeFunction();
            var step = new Step() { Function = new MethodInformation() { MethodName = "AsyncWait" } };
            var functionHarness = GetFunctionHarness(fakeFunction, step);
            await functionHarness.ExecuteAsync(cts.Token);
            
            fakeFunction.AsyncTask.IsCompleted.Should().BeTrue();
            AssertFunctionCall(fakeFunction, "AsyncWait");
        }
        
        [Fact]
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
            fakeFunction.CallList
                        .Any(c => c.AdditionalData == $"input1={uniqueString},input2={randomInt}")
                        .Should()
                        .BeTrue();
        }
        
        [Fact]
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
            fakeFunction.CallList
                        .Any(c => c.AdditionalData == $"input1={uniqueString},input2={randomInt},input3={uniqueString2}")
                        .Should()
                        .BeTrue();
        }
        
        [Fact]
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
            fakeFunction.CallList
                        .Any(c => c.AdditionalData == $"input1={uniqueString},input2={randomInt},input3=default")
                        .Should()
                        .BeTrue();
        }

        private void AssertFunctionCall(FakeFunction fakeFunction, string MethodName)
        {
            fakeFunction.CallList
                        .Count
                        .Should()
                        .Be(1);

            fakeFunction.CallList
                        .Any(c => c.InvocationTarget == MethodName)
                        .Should()
                        .BeTrue($"{MethodName} called");
        }
    }
}
