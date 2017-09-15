using System;
using System.Collections.Generic;
using Runic.Agent.Framework.Attributes;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.TestUtility
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

        [Function("FakeFunction")]
        public void DoFakeFunction()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "FakeFunction"
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

        [Function("Register")]
        public void DoSomeTask2()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "Register"
            });
        }

        [Function("ReturnFoo")]
        public string DoSomeTask3()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "ReturnFoo"
            });
            return "ReturnBar";
        }

        [Function("ReturnBar")]
        public string DoSomeTask4()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "ReturnBar"
            });
            return "ReturnFoo";
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

        [Function("InputsWithDefault")]
        public void DoSomeTask2(string input1, int input2, string defaultVal = "default")
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "InputsWithDefault",
                AdditionalData = $"input1={input1},input2={input2},input3={defaultVal}"
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
