using FluentAssertions;
using Moq;
using Runic.Cucumber.UnitTest.TestUtility;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Runic.Cucumber.UnitTest.Tests
{
    public class AssemblyAdapterTests : TestBase
    {
        Mock<FakeCucumberClass> _fakeTest { get; set; }
        
        public AssemblyAdapterTests()
        {
            _fakeTest = new Mock<FakeCucumberClass>();
            TestEnvironment.SetupMocks(_fakeTest);
        }
        
        [Fact]
        public async Task WhenGivenMethodIsExecuted_MethodIsInvoked()
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
        
        [Fact]
        public void WhenInitialised_PopulatesMethodReferences()
        {
            ((AssemblyAdapter)TestEnvironment.AssemblyAdapter.Instance).Methods.Count.Should().Be(6);
        }
        
        [Fact]
        public async Task WhenGivenMethodAttributeExecuted_LocatesAndInvokesMethod()
        {
            var statement = "Given I have a given \"method\"";
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            const string input = "input";
            await TestEnvironment.AssemblyAdapter
                                 .Instance
                                 .ExecuteMethodFromStatementAsync(statement, new object[] { input }, cts.Token);

            _fakeTest.Verify(f => f.GivenMethod(input));
        }

        [Fact]
        public async Task WhenDuplicateMethodExecuted_ThrowsException()
        {
            var statement = "Given I have a duplicate method";
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            await Assert.ThrowsAsync<MultipleMethodsFoundException>(async () => 
                await TestEnvironment.AssemblyAdapter
                                     .Instance
                                     .ExecuteMethodFromStatementAsync(statement, new object[] { }, cts.Token));
        }
        
        [Fact]
        public async Task WhenMethodNotFound_ThrowsException()
        {
            var fakeTest = new Mock<FakeCucumberClass>();
            TestEnvironment.SetupMocks(fakeTest);

            var statement = "Given I have a non existent method";
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            await Assert.ThrowsAsync<MethodNotFoundException>(async () =>
                await TestEnvironment.AssemblyAdapter
                                     .Instance
                                     .ExecuteMethodFromStatementAsync(statement, new object[] { }, cts.Token));
        }
    }
}
