﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Attributes;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Runic.Framework.Clients;
using Runic.Agent.Core.ExternalInterfaces;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class FunctionHarnessTests
    {
        [TestMethod]
        public void WhenGettingMethodWithAttribute_MethodIsFound()
        {
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness.FunctionHarness(new Mock<IStatsClient>().Object, new Mock<ILoggingHandler>().Object);
            functionHarness.Bind(fakeFunction, "step1", "Login", false);
            var method = functionHarness.GetMethodWithAttribute(typeof(BeforeEachAttribute));
            Assert.IsNotNull(method, "beforeeach method not found");
        }

        [TestMethod]
        public async Task WhenExecutingBeforeEach_MethodIsInvoked()
        {
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness.FunctionHarness(new Mock<IStatsClient>().Object, new Mock<ILoggingHandler>().Object);
            functionHarness.Bind(fakeFunction, "step1", "Login", false);
            var method = functionHarness.GetMethodWithAttribute(typeof(BeforeEachAttribute));
            var cts = new CancellationTokenSource();
            try
            {
                cts.CancelAfter(5000);
                var task = functionHarness.ExecuteMethodAsync(method, cts.Token);
                await task;
                Assert.IsNull(task.Exception);
            }
            catch (TaskCanceledException)
            {
                //all g
            }
        }

        [TestMethod]
        public async Task WhenExecutingFunction_MethodIsInvoked()
        {
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness.FunctionHarness(new Mock<IStatsClient>().Object, new Mock<ILoggingHandler>().Object);
            functionHarness.Bind(fakeFunction, "step1", "AsyncWait", false);
            var cts = new CancellationTokenSource();
            try
            {
                cts.CancelAfter(5000);
                var task = functionHarness.ExecuteFunctionAsync(cts.Token);
                await task;
                Assert.IsTrue(fakeFunction.AsyncTask.IsCompleted);
                Assert.IsNull(task.Exception);
            }
            catch (TaskCanceledException)
            {
                //all g
            }
        }

        [TestMethod]
        public async Task WhenAFunctionIsBoundAndExecuted_MethodsAreInvoked()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness.FunctionHarness(new Mock<IStatsClient>().Object, new Mock<ILoggingHandler>().Object);
            functionHarness.Bind(fakeFunction, "step1", "Login", false);
            var result = await functionHarness.OrchestrateFunctionExecutionAsync(cts.Token);
            //todo verify mock
            fakeFunction.CallList.ForEach(c => Console.WriteLine(c.InvocationTarget));
            Assert.IsTrue(result.Success, "Function returned false - error in execution");
            Assert.AreEqual(3, fakeFunction.CallList.Count);
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "BeforeEach"), "BeforeEach called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "Login"), "Login called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "AfterEach"), "AfterEach called");
        }

        [TestMethod]
        public async Task WhenAsyncFunctionIsCalled_HarnessWaitsToCompletion()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness.FunctionHarness(new Mock<IStatsClient>().Object, new Mock<ILoggingHandler>().Object);
            functionHarness.Bind(fakeFunction, "step1", "AsyncWait", false);
            await functionHarness.OrchestrateFunctionExecutionAsync(cts.Token);
            
            Assert.IsTrue(fakeFunction.AsyncTask.IsCompleted);
            Assert.AreEqual(3, fakeFunction.CallList.Count);
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "BeforeEach"), "BeforeEach called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "AsyncWait"), "AsyncWait called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "AfterEach"), "AfterEach called");
        }

        [TestMethod]
        public async Task WhenInputParametersAreBound_InputsArePassedToMethods()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness.FunctionHarness(new Mock<IStatsClient>().Object, new Mock<ILoggingHandler>().Object);

            var uniqueString = Guid.NewGuid().ToString("n");
            var randomInt = new Random().Next();
            functionHarness.Bind(fakeFunction, "step1", "Inputs", false, uniqueString, randomInt);
            await functionHarness.OrchestrateFunctionExecutionAsync(cts.Token);

            Assert.AreEqual(3, fakeFunction.CallList.Count);
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "BeforeEach"), "BeforeEach called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "Inputs"), "Inputs called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "AfterEach"), "AfterEach called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.AdditionalData == $"input1={uniqueString},input2={randomInt}"));
        }

        [TestMethod]
        public async Task WhenInputParametersAreBound_OverrideDefaultOveridesTheInput()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness.FunctionHarness(new Mock<IStatsClient>().Object, new Mock<ILoggingHandler>().Object);

            var uniqueString = Guid.NewGuid().ToString("n");
            var uniqueString2 = Guid.NewGuid().ToString("n");
            var randomInt = new Random().Next();

            functionHarness.Bind(fakeFunction, "step1", "InputsWithDefault", false, uniqueString, randomInt, uniqueString2);
            await functionHarness.OrchestrateFunctionExecutionAsync(cts.Token);

            Assert.AreEqual(3, fakeFunction.CallList.Count);
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "BeforeEach"), "BeforeEach called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "InputsWithDefault"), "InputsWithDefault called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "AfterEach"), "AfterEach called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.AdditionalData == $"input1={uniqueString},input2={randomInt},input3={uniqueString2}"));
        }

        [TestMethod]
        public async Task WhenInvokingMethodWithDefaults_MethodsAreInvokedWithDefault()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness.FunctionHarness(new Mock<IStatsClient>().Object, new Mock<ILoggingHandler>().Object);

            var uniqueString = Guid.NewGuid().ToString("n");
            var randomInt = new Random().Next();
            functionHarness.Bind(fakeFunction, "step1", "InputsWithDefault", false, uniqueString, randomInt);
            await functionHarness.OrchestrateFunctionExecutionAsync(cts.Token);

            Assert.AreEqual(3, fakeFunction.CallList.Count);
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "BeforeEach"), "BeforeEach called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "InputsWithDefault"), "InputsWithDefault called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "AfterEach"), "AfterEach called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.AdditionalData == $"input1={uniqueString},input2={randomInt},input3=default"));
        }
    }
}