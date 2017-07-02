using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.UnitTest.TestUtility;
using System;
using System.Linq;
using Runic.Agent.Core.ThreadManagement;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestPatternService
    {
        private TestEnvironment _testEnvironment { get; set; }
        private Flow _fakeFlow { get; set; }

        [TestInitialize]
        public void Init()
        {
            _testEnvironment = new TestEnvironment();
        
            _fakeFlow = TestData.GetTestFlowSingleStepLooping;
        }

        //[TestMethod]
        //public async Task TestSetThreadLevel()
        //{
        //    _testEnvironment.App.FlowManager.AddUpdateFlow(
        //        TestData.GetTestFlowSingleStepLooping);
        //    var executionContext = new ThreadManagement.ExecutionContext()
        //    {
        //        pluginManager = _testEnvironment.App.PluginManager,
        //        flowManager = _testEnvironment.App.FlowManager,
        //        stats = _testEnvironment.App.Stats,
        //        dataService = _testEnvironment.App.DataService
        //    };
        //    var agent = new PatternService(executionContext, new ThreadManager(executionContext));
        //
        //    var cts = new CancellationTokenSource();
        //    cts.CancelAfter(5000);
        //
        //    await _testEnvironment.App.SetThreadLevelAsync(new SetThreadLevelRequest()
        //    {
        //        FlowName = "Test",
        //        FlowId = "MyFlow",
        //        ThreadLevel = 1
        //    }, cts.Token);
        //
        //    Thread.Sleep(150);
        //    Assert.AreEqual(1, agent.GetThreadLevel("MyFlow"));
        //
        //    try
        //    {
        //        cts.Cancel();
        //    }
        //    catch (TaskCanceledException)
        //    {
        //        
        //    }
        //    catch (OperationCanceledException)
        //    {
        //
        //    }
        //}
    }
}
