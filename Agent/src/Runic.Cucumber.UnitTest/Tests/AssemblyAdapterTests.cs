using FluentAssertions;
using Moq;
using Runic.Cucumber.UnitTest.TestUtility;
using System.Reflection;
using System.Threading;
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
        public void WhenGivenMethodIsExecuted_MethodIsInvoked()
        {
            var method = _fakeTest.Object
                                  .GetType()
                                  .GetTypeInfo()
                                  .GetMethod("GivenMethod");

      
            const string input = "input";
            TestEnvironment.AssemblyAdapter
                           .Instance
                           .ExecuteMethod(_fakeTest.Object, method, new object[] { input });

            _fakeTest.Verify(f => f.GivenMethod(input));
        }
        
        [Fact]
        public void WhenInitialised_PopulatesMethodReferences()
        {
            ((AssemblyAdapter)TestEnvironment.AssemblyAdapter.Instance).Methods.Count.Should().Be(6);
        }
        
        [Fact]
        public void WhenGivenMethodAttributeExecuted_LocatesAndInvokesMethod()
        {
            var statement = "Given I have a given \"method\"";
            const string input = "input";
            TestEnvironment.AssemblyAdapter
                                 .Instance
                                 .ExecuteMethodFromStatement(statement, new object[] { input });

            _fakeTest.Verify(f => f.GivenMethod(input));
        }

        [Fact]
        public void WhenDuplicateMethodExecuted_ThrowsException()
        {
            var statement = "Given I have a duplicate method";
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            Assert.Throws<MultipleMethodsFoundException>(() => 
                TestEnvironment.AssemblyAdapter
                               .Instance
                               .ExecuteMethodFromStatement(statement, new object[] { }));
        }
        
        [Fact]
        public void WhenMethodNotFound_ThrowsException()
        {
            var fakeTest = new Mock<FakeCucumberClass>();
            TestEnvironment.SetupMocks(fakeTest);

            var statement = "Given I have a non existent method";
            Assert.Throws<MethodNotFoundException>(() =>
                TestEnvironment.AssemblyAdapter
                               .Instance
                               .ExecuteMethodFromStatement(statement, new object[] { }));
        }
    }
}
