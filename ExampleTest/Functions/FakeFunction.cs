using System;
using System.Collections.Generic;
using Runic.Core.Attributes;

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

        [Function("FakeAction")]
        public void DoSomeTask1()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "Login"
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
}
