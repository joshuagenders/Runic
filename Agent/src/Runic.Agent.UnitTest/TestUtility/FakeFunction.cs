using System;
using System.Collections.Generic;
using Runic.Framework.Attributes;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.UnitTest.TestUtility
{
    public class FakeFunction
    {
        public List<InvocationInformation> CallList { get; }
        public Task AsyncTask { get; set; }

        public FakeFunction()
        {
            CallList = new List<InvocationInformation>();
        }

        [BeforeEach]
        public void BeforeEach()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "BeforeEach"
            });
        }

        [Function("AsyncWait")]
        public async Task DoWait()
        {
            AsyncTask = Task.Run(() => Thread.Sleep(1500));
            await AsyncTask;

            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "AsyncWait"
            });
        }

        [Function("Login")]
        public void DoSomeTask1()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "Login"
            });
        }

        [Function("Inputs")]
        public void DoSomeTask1(string input1, int input2)
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "Inputs",
                AdditionalData = $"input1={input1},input2={input2}"
            });
        }

        [AfterEach]
        public void AfterEach()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "AfterEach"
            });
        }
    }
}
