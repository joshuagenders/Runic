using Moq;
using Xunit;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Configuration;
using System.Linq;
using FluentAssertions;

namespace Runic.Agent.Core.UnitTest.Tests
{
    public class AssemblyManagerTests
    {
        private AssemblyManager _assemblyManager { get; set; }

        public AssemblyManagerTests()
        {
            var config = new Mock<ICoreConfiguration>();
            config.Setup(c => c.PluginFolderPath).Returns("/plugins/");
            config.Setup(c => c.TaskCreationPollingIntervalSeconds).Returns(2);
            _assemblyManager = new AssemblyManager(config.Object);
        }
        
        [Fact]
        public void WhenLoadingMissingAssembly_ThrowsException()
        {
            Assert.Throws<AssemblyLoadException>(() => _assemblyManager.LoadAssembly("someassembly"));
        }

        [Fact]
        public void WhenGettingUnloadedAssembly_ThrowsException()
        {
            Assert.Throws<AssemblyLoadException>(() => _assemblyManager.GetAssembly("someassembly"));
        }
        
        [Fact]
        public void WhenGettingAssembliesWithoutLoad_ReturnsEmptyList()
        {
            _assemblyManager.GetAssemblies().Any().Should().BeFalse();
        }
        
        [Fact]
        public void WhenGettingAssemblyKeysWithoutLoad_ReturnsEmptyList()
        {
            _assemblyManager.GetAssemblyKeys().Any().Should().BeFalse();
        }
        
        [Fact]
        public void WhenGettingMethodsWithoutLoad_ReturnsEmptyList()
        {
            _assemblyManager.GetAvailableMethods().Any().Should().BeFalse();
        }
    }
}
