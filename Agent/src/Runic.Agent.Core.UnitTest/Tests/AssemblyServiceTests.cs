using Xunit;
using Runic.Agent.Core.Services;
using System;

namespace Runic.Agent.Core.UnitTest.Tests
{
    public class AssemblyServiceTests
    {
        private AssemblyService _assemblyService { get; set; }

        public AssemblyServiceTests()
        {
            _assemblyService = new AssemblyService();
        }
        
        [Fact]
        public void WhenLoadingMissingAssembly_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => _assemblyService.GetLoadAssembly("someassembly"));
        }
    }
}
