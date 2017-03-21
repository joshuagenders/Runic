using System;
using System.Collections.Generic;
using Runic.Framework.Attributes;
using System.Threading.Tasks;
using System.Threading;

namespace Runic.ExampleTest.Functions
{
    public class FakeFunction
    {
        public List<InvocationInformation> CallList { get; }

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
        public async Task Wait()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "AsyncWait"
            });
            await Task.Run(() => Thread.Sleep(500));
        }

        [Function("FakeAction")]
        public void DoSomeTask1()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "FakeAction"
            });
        }

        [Function("Add")]
        public void DoSomeTask2()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "Add"
            });
        }

        [Function("Subtract")]
        public void DoSomeTask3()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "Subtract"
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
    public class InvocationInformation
    {
        public DateTime InvocationTime { get; set; }
        public string StackTrace { get; set; }
        public string InvocationTarget { get; set; }
    }
}
