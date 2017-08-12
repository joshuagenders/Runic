using Runic.Framework.Attributes;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public class FakeFunction
    {
        //private List<>

        [BeforeEach]
        public virtual void BeforeEach()
        {
        }

        [Function("AsyncWait")]
        public virtual async Task DoWait()
        {
            await Task.CompletedTask;
        }

        [Function("Login")]
        public virtual void DoSomeTask1()
        {
        }

        [Function("Inputs")]
        public virtual void Inputs(string input1, int input2)
        {
        }

        [Function("InputsWithDefault")]
        public virtual void DoSomeTask2(string input1, int input2, string defaultVal = "default")
        {
        }

        [AfterEach]
        public virtual void AfterEach()
        {
        }
    }
}
