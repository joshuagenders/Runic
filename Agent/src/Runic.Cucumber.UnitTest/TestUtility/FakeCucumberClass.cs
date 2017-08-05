using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runic.Cucumber.UnitTest.TestUtility
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

        [Given("I have a method with no inputs")]
        public void NoInputs()
        {
            CallList.Add(new InvocationInformation()
            {
                InvocationTime = DateTime.Now,
                StackTrace = Environment.StackTrace,
                InvocationTarget = "NoInputs"
            });
        }
    }
}
