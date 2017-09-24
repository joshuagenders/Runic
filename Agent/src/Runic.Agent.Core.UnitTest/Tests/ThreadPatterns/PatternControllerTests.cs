using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.ThreadPatterns
{
    [TestClass]
    public class PatternControllerTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenRunningPattern_ThenThreadLevelsAreSet()
        {
            var datetimeService = new Mock<IDatetimeService>();
            var threadManager = new Mock<IPopulationControl>();
            var pattern = new Mock<IPopulationPattern>();

            var flow = TestData.GetTestFlowSingleStep;
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1200);

            var patternController = new PopulationPatternController(datetimeService.Object, threadManager.Object);
            patternController.AddPopulationPattern("id", flow, pattern.Object, cts.Token);

            Assert.IsTrue(patternController.GetRunningThreadPatterns().Any(p => p.Item1 == "id"));
            Assert.IsTrue(patternController.GetRunningFlows().Any(p => p.Item1 == "id"));

            pattern.Setup(p => p.GetCurrentActivePopulationCount(It.IsAny<DateTime>())).Returns(20);
            //todo idatetime setup, use callback with semaphore
            var controllerTask = patternController.Run(cts.Token);
            threadManager.Verify(t => t.SetThreadLevelAsync(It.IsAny<string>(), It.IsAny<Framework.Models.Journey>(), 20, It.IsAny<CancellationToken>()));

            pattern.Setup(p => p.GetCurrentActivePopulationCount(It.IsAny<DateTime>())).Returns(0);
            await controllerTask;

            threadManager.Verify(t => t.SetThreadLevelAsync(It.IsAny<string>(), It.IsAny<Framework.Models.Journey>(), 0, It.IsAny<CancellationToken>()));

            Assert.IsFalse(patternController.GetRunningThreadPatterns().Any(p => p.Item1 == "id"));
            Assert.IsFalse(patternController.GetRunningFlows().Any(p => p.Item1 == "id"));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenStoppingPattern_ThenFlowAndPatternStops()
        {
            var datetimeService = new Mock<IDatetimeService>();
            var threadManager = new Mock<IPopulationControl>();
            var pattern = new Mock<IPopulationPattern>();

            var flow = TestData.GetTestFlowSingleStep;
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1200);

            var patternController = new PopulationPatternController(datetimeService.Object, threadManager.Object);
            patternController.AddPopulationPattern("id", flow, pattern.Object, cts.Token);

            Assert.IsTrue(patternController.GetRunningThreadPatterns().Any(p => p.Item1 == "id"));
            Assert.IsTrue(patternController.GetRunningFlows().Any(p => p.Item1 == "id"));
            pattern.Setup(p => p.GetCurrentActivePopulationCount(It.IsAny<DateTime>())).Returns(20);

            var controllerTask = patternController.Run(cts.Token);
            threadManager.Verify(t => t.SetThreadLevelAsync(It.IsAny<string>(), It.IsAny<Framework.Models.Journey>(), 20, It.IsAny<CancellationToken>()));

            await patternController.StopAll(cts.Token);
            await controllerTask;

            threadManager.Verify(t => t.SetThreadLevelAsync(It.IsAny<string>(), It.IsAny<Framework.Models.Journey>(), 0, It.IsAny<CancellationToken>()));
            Assert.IsFalse(patternController.GetRunningThreadPatterns().Any(p => p.Item1 == "id"));
            Assert.IsFalse(patternController.GetRunningFlows().Any(p => p.Item1 == "id"));
        }
    }
}
