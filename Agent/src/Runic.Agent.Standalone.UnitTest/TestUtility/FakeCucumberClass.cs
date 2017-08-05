using Runic.Cucumber;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public class FakeCucumberClass
    {
        public List<InvocationInformation> CallList { get; }
        public Task AsyncTask { get; set; }
        public static ConcurrentDictionary<DateTime, FakeCucumberClass> CreatedInstances { get; set; } = new ConcurrentDictionary<DateTime, FakeCucumberClass>();
        
        public FakeCucumberClass()
        {
            CallList = new List<InvocationInformation>();
            CreatedInstances.AddOrUpdate(DateTime.Now, this, (a, b) => this);
        }

        [Given("I have a given \"(.*)\"")]
        public void GivenMethod(string input)
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "GivenMethod",
                AdditionalData = input
            });
        }

        [When("I have a when \"(.*)\"")]
        public void WhenMethod(string input)
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "WhenMethod",
                AdditionalData = input
            });
        }

        [Then("I have a then \"(.*)\"")]
        public void ThenMethod(string input)
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "ThenMethod",
                AdditionalData = input
            });
        }
    }
}
