using Xunit;
using Runic.Agent.Core.AssemblyManagement;
using System.Linq;
using FluentAssertions;
using System;

namespace Runic.Agent.Core.UnitTest.Tests
{
    public class AssemblyManagerTests
    {
        private AssemblyManager _assemblyManager { get; set; }

        public AssemblyManagerTests()
        {
            _assemblyManager = new AssemblyManager();
        }
        
        [Fact]
        public void WhenLoadingMissingAssembly_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => _assemblyManager.GetLoadAssembly("someassembly"));
        }
        
        [Fact]
        public void WhenGettingAssembliesWithoutLoad_ReturnsEmptyList()
        {
            _assemblyManager.GetCachedAssemblies().Any().Should().BeFalse();
        }
        
        [Fact]
        public void WhenGettingAssemblyKeysWithoutLoad_ReturnsEmptyList()
        {
            _assemblyManager.GetCachedAssemblyKeys().Any().Should().BeFalse();
        }
        
        [Fact]
        public void WhenGettingMethodsWithoutLoad_ReturnsEmptyList()
        {
            _assemblyManager.GetAvailableMethods().Any().Should().BeFalse();
        }
    }
}
