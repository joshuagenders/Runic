using System.Linq;
using System.Threading.Tasks;
using Runic.Agent.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestFunctionHarness
    {
        [TestMethod]
        public async Task TestBindAndExecute()
        {
            var fakeFunction = new FakeFunction();
            var functionHarness = new FunctionHarness();
            functionHarness.Bind(fakeFunction, "Login");
            await functionHarness.Execute();
            Assert.AreEqual(fakeFunction.CallList.Count, 3);
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "BeforeEach"));
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "Login"));
            Assert.IsTrue(fakeFunction.CallList.Any(c => c.InvocationTarget == "AfterEach"));
        }
    }
}
