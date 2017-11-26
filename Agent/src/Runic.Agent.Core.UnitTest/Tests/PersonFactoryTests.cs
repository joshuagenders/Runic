using FluentAssertions;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using Xunit;

namespace Runic.Agent.Core.UnitTest.Tests
{
    public class PersonFactoryTests
    {
        [Fact]
        public void WhenPersonRequested_ThenPersonReturned()
        {
            var ff = new Mock<IFunctionFactory>();
            var dts = new Mock<IDatetimeService>();
            var am = new Mock<IAssemblyManager>();
            var personFactory = new PersonFactory(ff.Object, dts.Object, am.Object);
            var journey = new Journey()
            {
                Name = "Test Journey"
            };
            personFactory.GetPerson(journey).GetType().Should().Be(typeof(Person));
        }
    }
}
