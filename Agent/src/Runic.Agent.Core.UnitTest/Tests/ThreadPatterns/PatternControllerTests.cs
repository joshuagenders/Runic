using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests.ThreadPatterns
{
    [TestClass]
    public class PatternControllerTests
    {
        [TestMethod]
        public async Task WhenRunningPattern_ThenThreadLevelsAreSet()
        {
            var datetimeService = new Mock<IDatetimeService>();
            var startTime = new DateTime();
            datetimeService.Setup(s => s.Now).Returns(startTime);
            var threadManager = new Mock<IThreadManager>();
            var patternController = new PatternController(datetimeService.Object, threadManager.Object);
            var flow = TestData.GetTestFlowSingleStep;
            var pattern = new Mock<IThreadPattern>();

            var cts = new CancellationTokenSource();
            cts.CancelAfter(1200);

            patternController.StartThreadPattern("id", flow, pattern.Object, cts.Token);
            Assert.IsTrue(patternController.GetRunningThreadPatterns().Any(p => p.Item1 == "id"));
            Assert.IsTrue(patternController.GetRunningFlows().Any(p => p.Item1 == "id"));

            pattern.Setup(p => p.GetCurrentThreadLevel(It.IsAny<DateTime>())).Returns(20);
            var controllerTask = patternController.Run(cts.Token);

            threadManager.Verify(t => t.SetThreadLevelAsync(It.Is<SetThreadLevelRequest>(r => r.ThreadLevel == 20), It.IsAny<CancellationToken>()));

            pattern.Setup(p => p.GetCurrentThreadLevel(It.IsAny<DateTime>())).Returns(0);
            await controllerTask;
            Assert.IsFalse(patternController.GetRunningThreadPatterns().Any(p => p.Item1 == "id"));
            Assert.IsFalse(patternController.GetRunningFlows().Any(p => p.Item1 == "id"));
        }

        [TestMethod]
        public void WhenStoppingPattern_ThenFlowAndPatternStop()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void WhenStoppingFlow_ThenFlowAndPatternStop()
        {
            throw new NotImplementedException();
        }
    }
}
