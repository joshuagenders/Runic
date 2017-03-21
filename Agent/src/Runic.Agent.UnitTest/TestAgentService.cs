using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.FlowManagement;
using Runic.Agent.Service;
using Runic.Framework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.AssemblyManagement;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestAgentService
    {
        private AgentWorld _world { get; set; }

        [TestInitialize]
        public void Init()
        {
            _world = new AgentWorld();
        }
        //[TestMethod]
        //[TestCategory("SystemTest")]
        public void TestWikipedia()
        {
            var flows = new Flows();
            flows.AddUpdateFlow(new Flow()
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

            var agent = new AgentService(new PluginManager(), flows);

            agent.StartFlow(new FlowContext()
            {
                FlowName = "Wikipedia Flow",
                Flow = flows.GetFlow("Wikipedia Flow"),
                ThreadCount = 1
            });
            Thread.Sleep(5000);
        }

        [TestMethod]
        public void TestStartThread()
        {
            var flows = new Flows();
            flows.AddUpdateFlow(new Flow()
            {
                Name = "FakeFlow",
                StepDelayMilliseconds = 200,
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step1", 
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.ExampleTest",
                            AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.FakeFunction",
                            FunctionName = "FakeFunction"
                        },
                        NextStepOnFailure = "Step1",
                        NextStepOnSuccess = "Step1"
                    }
                }
            });

            var agent = new AgentService(new PluginManager(), new Flows());

            agent.StartFlow(new FlowContext()
            {
                FlowName = "FakeFlow",
                Flow = flows.GetFlow("FakeFlow"),
                ThreadCount = 1
            });

            Assert.AreEqual(1, agent.GetThreadLevel("FakeFlow"));
        }

        [TestMethod]
        public async Task TestSetThreadLevel()
        {
            var flows = new Flows();
            
            flows.AddUpdateFlow(
                new Flow()
                {
                    Name = "FakeFlow",
                    StepDelayMilliseconds = 200,
                    Steps = new List<Step>()
                    {
                        new Step()
                        {
                            StepName = "Step1",
                            Function = new FunctionInformation()
                            {
                                AssemblyName = "Runic.ExampleTest",
                                AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.FakeFunction",
                                FunctionName = "FakeFunction"
                            },
                            NextStepOnFailure = "Step1",
                            NextStepOnSuccess = "Step1"
                        }
                    }
                });

            var agent = new AgentService(new PluginManager(), flows);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);

            var agentTask = agent.SetThreadLevel(new SetThreadLevelRequest()
            {
                FlowName = "FakeFlow",
                ThreadLevel = 1
            }, cts.Token);

            Thread.Sleep(80);
            Assert.AreEqual(1, agent.GetThreadLevel("FakeFlow"));

            try
            {
                cts.Cancel();
                await agentTask;
            }
            catch (TaskCanceledException)
            {
                
            }
        }

        [TestMethod]
        public void TestExecutionContext()
        {
            var executionContext = new Service.ExecutionContext();
            Assert.IsTrue(executionContext.MaxThreadCount > 0);
        }
    }
}
