using Runic.Cucumber;

namespace Runic.Agent.TestUtility.Cucumber
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
        [Given("I have a method with no inputs")]
        public virtual void NoInputs()
        {
        }
        [Given("I have a duplicate method")]
        public virtual void DuplicateMethod1()
        {
        }
        [Given("I have a duplicate method")]
        public virtual void DuplicateMethod2()
        {
        }
    }
}
