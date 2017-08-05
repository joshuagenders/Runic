using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Cucumber.UnitTest.TestUtility;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Cucumber.UnitTest
{
    [TestClass]
    public class AssemblyAdapterTests : TestBase
    {
        [TestMethod]
        public async Task MethodExecutionShouldCallMethods()
        {
            var fakeTest = new FakeCucumberClass();
            TestEnvironment.SetupMocks(fakeTest);
            var method = fakeTest.GetType()
                                 .GetTypeInfo()
                                 .GetMethod("GivenMethod");

            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            await TestEnvironment.AssemblyAdapter
                                 .Instance
                                 .ExecuteMethodAsync(fakeTest, method, new object[] { "" }, cts.Token);

            fakeTest.CallList
                    .Count(c => c.InvocationTarget == "GivenMethod")
                    .Should()
                    .Be(1);
        }

        [TestMethod]
        public void InitialiseShouldPopulateMethodReferences()
        {
            var fakeTest = new FakeCucumberClass();
            TestEnvironment.SetupMocks(fakeTest);
            ((AssemblyAdapter)TestEnvironment.AssemblyAdapter.Instance).Methods.Count.Should().Be(4);
        }

        [TestMethod]
        public async Task AttributeExecutionShouldCallMethods()
        {
            var fakeTest = new FakeCucumberClass();
            TestEnvironment.SetupMocks(fakeTest);

            var method = fakeTest.GetType()
                                 .GetTypeInfo()
                                 .GetMethod("GivenMethod");

            var statement = "Given I have a given \"method\"";
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            await TestEnvironment.AssemblyAdapter
                                 .Instance
                                 .ExecuteMethodFromStatementAsync(statement, new object[] { "" }, cts.Token);
            
            fakeTest.CallList.Count(c => c.InvocationTarget == "GivenMethod").Should().Be(1);
        }
    }
}
