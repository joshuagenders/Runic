using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.FlowManagement;
using Runic.Agent.Service;
using Runic.Agent.UnitTest.TestUtility;
using Runic.Framework.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.UnitTest.Tests
{
    public class TestWikipedia
    {
        private AgentWorld _world { get; set; }

        [TestInitialize]
        public void Init()
        {
            _world = new AgentWorld();
        }

        [TestMethod]
        [TestCategory("SystemTest")]
        public async Task RunTestWikipedia()
        {
            _world.FlowManager.AddUpdateFlow(new Flow()
            {
                Name = "Wikipedia Flow",
                StepDelayMilliseconds = 700,
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "GetHome",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.ExampleTest",
                            AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.ViewHomepageFunction",
                            FunctionName = "OpenFirstLinkFunction"
                        },
                        NextStepOnFailure = "GetHome",
                        NextStepOnSuccess = "Search"
                    },
                    new Step()
                    {
                        StepName = "Search",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.ExampleTest",
                            AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.SearchFunction",
                            FunctionName = "OpenFirstLinkFunction"
                        },
                        NextStepOnFailure = "GetHome",
                        NextStepOnSuccess = "OpenLink"
                    },
                    new Step()
                    {
                        StepName = "OpenLink",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.ExampleTest",
                            AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.OpenFirstLinkFunction",
                            FunctionName = "OpenFirstLinkFunction"
                        },
                        NextStepOnFailure = "Search",
                        NextStepOnSuccess = "OpenLink"
                    }
                }
            });

            var agent = new AgentService(_world.PluginManager, _world.MessagingService, _world.FlowManager, _world.Stats, _world.DataService);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            await agent.SetThreadLevel(new SetThreadLevelRequest()
            {
                FlowName = "Wikipedia Flow",
                ThreadLevel = 5
            }, cts.Token);
        }
    }
}
