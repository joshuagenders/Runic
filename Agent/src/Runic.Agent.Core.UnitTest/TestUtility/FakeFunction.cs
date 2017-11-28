using System;
using System.Collections.Generic;
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
        
        public void BeforeEach()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "BeforeEach"
            });
        }
        
        public async Task AsyncWait()
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

        public void Login()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "Login"
            });
        }

        public void Register()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "Register"
            });
        }

        public string ReturnFoo()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "ReturnFoo"
            });
            return "ReturnBar";
        }
        
        public string ReturnBar()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "ReturnBar"
            });
            return "ReturnFoo";
        }

        public void Inputs(string input1, int input2)
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "Inputs",
                AdditionalData = $"input1={input1},input2={input2}"
            });
        }

        public void NoInputs()
        {
        }

        public void SingleStringInput(string input)
        {
        }

        public void DefaultStringInput(string input = "default")
        {
        }

        public void InputsWithDefault(string input1, int input2, string defaultVal = "default")
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "InputsWithDefault",
                AdditionalData = $"input1={input1},input2={input2},input3={defaultVal}"
            });
        }
        
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
