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
        public void Initialise_PopulatesMethodReferences()
        {
            var fakeTest = new FakeCucumberClass();
            TestEnvironment.SetupMocks(fakeTest);
            ((AssemblyAdapter)TestEnvironment.AssemblyAdapter.Instance).Methods.Count.Should().Be(6);
        }

        [TestMethod]
        public async Task GivenMethodAttribute_LocatesAndInvokesMethod()
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

        [TestMethod]
        public async Task MultipleMethods_ThrowsException()
        {
            var fakeTest = new FakeCucumberClass();
            TestEnvironment.SetupMocks(fakeTest);

            var method = fakeTest.GetType()
                                 .GetTypeInfo()
                                 .GetMethod("GivenMethod");

            var statement = "Given I have a duplicate method";
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            try
            {
                await TestEnvironment.AssemblyAdapter
                                     .Instance
                                     .ExecuteMethodFromStatementAsync(statement, new object[] { }, cts.Token);
                Assert.Fail("No exception thrown");
            }
            catch (MultipleMethodsFoundException)
            {
            }
        }

        [TestMethod]
        public async Task MethodNotFound_ThrowsException()
        {
            //todo refactor, consider testinit in testbase
            var fakeTest = new FakeCucumberClass();
            TestEnvironment.SetupMocks(fakeTest);

            var method = fakeTest.GetType()
                                 .GetTypeInfo()
                                 .GetMethod("GivenMethod");

            var statement = "Given I have a non existent method";
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            try
            {
                await TestEnvironment.AssemblyAdapter
                                     .Instance
                                     .ExecuteMethodFromStatementAsync(statement, new object[] { }, cts.Token);
                Assert.Fail("No exception thrown");
            }
            catch (MethodNotFoundException)
            {
            }
        }
    }
}
