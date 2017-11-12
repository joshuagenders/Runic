using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.UnitTest.TestUtility;

namespace Runic.Agent.Core.UnitTest.Tests.FlowManagement
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
            var flow = TestData.NewJourney.WithStep();   
            var journeyInitialiser = new JourneyInitialiser(_assemblyManager.Object);
            journeyInitialiser.InitialiseJourney(flow);
            _assemblyManager.Verify(p => p.LoadAssembly("AssemblyName"));
        }
    }
}
