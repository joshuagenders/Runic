using System.Linq;
using System.Threading.Tasks;
using Runic.Agent.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System;
using Runic.Framework.Attributes;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestFunctionHarness
    {
        [TestMethod]
        public void TestGetMethodWithAttribute()
        {
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness();
            functionHarness.Bind(fakeFunction, "Login");
            var method = functionHarness.GetMethodWithAttribute(typeof(BeforeEachAttribute));
            Assert.IsNotNull(method, "beforeeach method not found");
        }

        [TestMethod]
        public async Task TestBeforeEachExecute()
        {
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness();
            functionHarness.Bind(fakeFunction, "Login");
            var method = functionHarness.GetMethodWithAttribute(typeof(BeforeEachAttribute));
            var cts = new CancellationTokenSource();
            try
            {
                cts.CancelAfter(5000);
                var task = functionHarness.ExecuteMethod(method, cts.Token);
                await task;
                Assert.IsNull(task.Exception);
            }
            catch (TaskCanceledException)
            {
                //all g
            }
        }

        [TestMethod]
        public async Task TestFunctionExecute()
        {
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness();
            functionHarness.Bind(fakeFunction, "AsyncWait");
            var cts = new CancellationTokenSource();
            try
            {
                cts.CancelAfter(5000);
                var task = functionHarness.ExecuteFunction(cts.Token);
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
        public async Task TestBindAndExecute()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness();
            functionHarness.Bind(fakeFunction, "Login");
            var result = await functionHarness.Execute(cts.Token);

            fakeFunction.CallList.ForEach(c => Console.WriteLine(c.InvocationTarget));
            Assert.IsTrue(result, "Function returned false - error in execution");
            Assert.AreEqual(3, fakeFunction.CallList.Count);
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "BeforeEach"), "BeforeEach called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "Login"), "Login called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "AfterEach"), "AfterEach called");
        }

        [TestMethod]
        public async Task TestAsyncFunctionWaitsUntilCompletion()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness();
            functionHarness.Bind(fakeFunction, "AsyncWait");
            await functionHarness.Execute(cts.Token);
            
            Assert.IsTrue(fakeFunction.AsyncTask.IsCompleted);
            Assert.AreEqual(3, fakeFunction.CallList.Count);
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "BeforeEach"), "BeforeEach called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "AsyncWait"), "AsyncWait called");
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "AfterEach"), "AfterEach called");
        }
    }
}
