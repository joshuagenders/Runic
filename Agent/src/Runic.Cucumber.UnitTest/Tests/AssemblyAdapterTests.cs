using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Cucumber.UnitTest.TestUtility;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Cucumber.UnitTest.Tests
{
    [TestClass]
    public class AssemblyAdapterTests : TestBase
    {
        Mock<FakeCucumberClass> _fakeTest { get; set; }

        [TestInitialize]
        public override void Init()
        {
            base.Init();
            _fakeTest = new Mock<FakeCucumberClass>();
            TestEnvironment.SetupMocks(_fakeTest);
        }

        [TestMethod]
        public async Task MethodExecutionShouldCallMethods()
        {
            var method = _fakeTest.Object
                                  .GetType()
                                  .GetTypeInfo()
                                  .GetMethod("GivenMethod");

            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            const string input = "input";
            await TestEnvironment.AssemblyAdapter
                                 .Instance
                                 .ExecuteMethodAsync(_fakeTest.Object, method, new object[] { input }, cts.Token);

            _fakeTest.Verify(f => f.GivenMethod(input));
        }

        [TestMethod]
        public void Initialise_PopulatesMethodReferences()
        {
            ((AssemblyAdapter)TestEnvironment.AssemblyAdapter.Instance).Methods.Count.Should().Be(6);
        }

        [TestMethod]
        public async Task GivenMethodAttribute_LocatesAndInvokesMethod()
        {
            var method = _fakeTest.GetType()
                                 .GetTypeInfo()
                                 .GetMethod("GivenMethod");

            var statement = "Given I have a given \"method\"";
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            const string input = "input";
            await TestEnvironment.AssemblyAdapter
                                 .Instance
                                 .ExecuteMethodFromStatementAsync(statement, new object[] { input }, cts.Token);

            _fakeTest.Verify(f => f.GivenMethod(input));
        }

        [TestMethod]
        public async Task MultipleMethods_ThrowsException()
        {
            var method = _fakeTest.GetType()
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
            var fakeTest = new Mock<FakeCucumberClass>();
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
