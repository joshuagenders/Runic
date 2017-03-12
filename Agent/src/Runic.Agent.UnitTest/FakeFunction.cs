using System;
using System.Collections.Generic;
using Runic.Framework.Attributes;

namespace Runic.Agent.UnitTest
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
