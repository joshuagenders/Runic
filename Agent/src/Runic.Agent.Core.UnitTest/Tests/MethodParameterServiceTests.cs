using FluentAssertions;
using Runic.Agent.Core.Services;
using Runic.Agent.UnitTest.TestUtility;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Runic.Agent.Core.UnitTest.Tests
{
    public class MethodParameterServiceTests
    {
        [Fact]
        public void WhenNoInputs_ThenEmptyArrayReturned()
        {
            var mps = new MethodParameterService();
            var parms = mps.GetParams(new string [] { }, typeof(FakeFunction).GetTypeInfo().GetMethod("NoInputs", new Type [] { }));
            parms.Should().BeEmpty();
        }
        [Fact]
        public void WhenSingleStringInput_ThenNullReturned()
        {
            var mps = new MethodParameterService();
            var parms = mps.GetParams(new string[] { }, typeof(FakeFunction).GetTypeInfo().GetMethod("SingleStringInput", new Type[] { typeof(string) }));
            parms.Count().Should().Be(1);
            parms.Single().Should().BeNull();
        }
        [Fact]
        public void WhenSingleStringWithDefaulInput_ThenDefaultReturned()
        {
            var mps = new MethodParameterService();
            var parms = mps.GetParams(new string[] { }, typeof(FakeFunction).GetTypeInfo().GetMethod("DefaultStringInput", new Type[] { typeof(string) }));
            parms.Count().Should().Be(1);
            parms.Single().Should().Be("default");
        }
    }
}
