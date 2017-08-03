using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runic.Cucumber.UnitTest
{
    public class FakeCucumberClass
    {
        public List<InvocationInformation> CallList { get; }
        public Task AsyncTask { get; set; }

        public FakeCucumberClass()
        {
            CallList = new List<InvocationInformation>();
        }

        [Given("I have a given \"(.*)\"")]
        public void GivenMethod()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "GivenMethod"
            });
        }

        [When("I have a when \"(.*)\"")]
        public void WhenMethod()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "WhenMethod"
            });
        }

        [Then("I have a then \"(.*)\"")]
        public void ThenMethod()
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
