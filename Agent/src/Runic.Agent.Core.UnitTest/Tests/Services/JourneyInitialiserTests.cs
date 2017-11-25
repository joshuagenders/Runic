using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.UnitTest.TestUtility;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class JourneyInitialiserTests
    {
        private Mock<IAssemblyManager> _assemblyManager { get; set; }

        [TestInitialize]
        public void Init()
        {
            _assemblyManager = new Mock<IAssemblyManager>();
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenInitialiseJourney_PluginsAreLoaded()
        {
            var journey = TestData.NewJourney.WithStep();   
            JourneyInitialiserService.InitialiseJourney(_assemblyManager.Object, journey);
            _assemblyManager.Verify(p => p.LoadAssembly("AssemblyName"));
        }
    }
}
