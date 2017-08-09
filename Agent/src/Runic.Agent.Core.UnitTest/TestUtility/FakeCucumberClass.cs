using Runic.Cucumber;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.TestUtility
{
    public class FakeCucumberClass
    {
        public static ConcurrentDictionary<DateTime, FakeCucumberClass> FakeCucumberClasses { get; private set; } = new ConcurrentDictionary<DateTime, FakeCucumberClass>(); 
        public List<InvocationInformation> CallList { get; }
        public Task AsyncTask { get; set; }

        public FakeCucumberClass()
        {
            CallList = new List<InvocationInformation>();
            FakeCucumberClasses.AddOrUpdate(DateTime.Now, this, (a,b) => this);
        }

        [Given("I have a given \"(.*)\"")]
        public void GivenMethod(string input)
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "GivenMethod"
            });
        }

        [When("I have a when \"(.*)\"")]
        public void WhenMethod(string input)
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "WhenMethod"
            });
        }

        [Then("I have a then \"(.*)\"")]
        public void ThenMethod(string input)
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "ThenMethod"
            });
        }
    }
}
