using Runic.Cucumber;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public class FakeCucumberClass
    {
        [Given("I have a given \"(.*)\"")]
        public virtual void GivenMethod(string input)
        {
        }

        [When("I have a when \"(.*)\"")]
        public virtual void WhenMethod(string input)
        {
        }

        [Then("I have a then \"(.*)\"")]
        public virtual void ThenMethod(string input)
        {
        }
    }
}
