using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Runic.Agent.Harness;

namespace Runic.Agent.UnitTest
{
    public class TestFunctionHarness
    {
        [Test]
        public async Task TestBindAndExecute()
        {
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness();
            functionHarness.Bind(fakeFunction);
            await functionHarness.Execute("Login");
            Assert.AreEqual(fakeFunction.CallList.Count, 3);
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "BeforeEach"));
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "Login"));
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "AfterEach"));
        }
    }
}
