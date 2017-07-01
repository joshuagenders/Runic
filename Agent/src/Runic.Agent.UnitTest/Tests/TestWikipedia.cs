using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.UnitTest.TestUtility;
using Runic.Framework.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestWikipedia
    {
        private TestEnvironment _testEnvironment { get; set; }

        [TestInitialize]
        public void Init()
        {
            _testEnvironment = new TestEnvironment();
        }

        //[TestMethod]
        //[Ignore]
        //[TestCategory("SystemTest")]
        //public async Task RunTestWikipedia()
        //{
        //    _testEnvironment.App.FlowManager.AddUpdateFlow(new Flow()
        //    {
        //        Name = "Wikipedia Flow",
        //        StepDelayMilliseconds = 700,
        //        Steps = new List<Step>()
        //        {
        //            new Step()
        //            {
        //                StepName = "GetHome",
        //                Function = new FunctionInformation()
        //                {
        //                    AssemblyName = "Runic.ExampleTest",
        //                    AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.ViewHomepageFunction",
        //                    FunctionName = "OpenFirstLinkFunction"
        //                }
        //            },
        //            new Step()
        //            {
        //                StepName = "Search",
        //                Function = new FunctionInformation()
        //                {
        //                    AssemblyName = "Runic.ExampleTest",
        //                    AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.SearchFunction",
        //                    FunctionName = "OpenFirstLinkFunction"
        //                }
        //            },
        //            new Step()
        //            {
        //                StepName = "OpenLink",
        //                Function = new FunctionInformation()
        //                {
        //                    AssemblyName = "Runic.ExampleTest",
        //                    AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.OpenFirstLinkFunction",
        //                    FunctionName = "OpenFirstLinkFunction"
        //                }
        //            }
        //        }
        //    });
        //
        //    var agent = new PatternService(
        //        _testEnvironment.App.PluginManager,
        //        _testEnvironment.App.FlowManager,
        //        _testEnvironment.App.Stats,
        //        _testEnvironment.App.DataService);
        //
        //    CancellationTokenSource cts = new CancellationTokenSource();
        //    cts.CancelAfter(5000);
        //    await agent.SetThreadLevelAsync(new SetThreadLevelRequest()
        //    {
        //        FlowName = "Wikipedia Flow",
        //        ThreadLevel = 5
        //    }, cts.Token);
        //}
    }
}
