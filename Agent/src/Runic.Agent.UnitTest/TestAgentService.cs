using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Configuration;
using Runic.Agent.FlowManagement;
using Runic.Agent.Service;
using Runic.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestAgentService
    {
        [TestMethod]
        public void TestStartThread()
        {
            var cli = new[]
            {
                "Agent:MaxThreads=321",
                "Agent:LifetimeSeconds=123",
                "Client:MQConnectionString=MyExampleConnection",
                "Statsd:Port=8125",
                "Statsd:Host=localhost",
                "Statsd:Prefix=Runic.Stats."
            };
            AgentConfiguration.LoadConfiguration(cli);
            IoC.RegisterDependencies(new Startup());

            Flows.AddUpdateFlow(new Flow()
            {
                Name = "FakeFlow",
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        FunctionName = "FakeFunction",
                        FunctionAssemblyName = "ExampleTest",
                        FunctionFullyQualifiedName = "Runic.ExampleTest.Functions.FakeFunction"
                    }
                }
            });


            var agent = new AgentService();
            agent.StartFlow(new FlowContext()
            {
                FlowName = "FakeFlow",
                Flow = Flows.GetFlow("FakeFlow"),
                ThreadCount = 1
            });

            Assert.AreEqual(1, agent.GetThreadLevel("FakeFlow"));
        }

        [TestMethod]
        public async Task TestSetThreadLevel()
        {
            var cli = new[]
            {
                "Agent:MaxThreads=321",
                "Agent:LifetimeSeconds=123",
                "Client:MQConnectionString=MyExampleConnection",
                "Statsd:Port=8125",
                "Statsd:Host=localhost",
                "Statsd:Prefix=Runic.Stats."
            };
            AgentConfiguration.LoadConfiguration(cli);
            IoC.RegisterDependencies(new Startup());

            var agent = new AgentService();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            Flows.AddUpdateFlow(new Flow()
            {
                Name = "FakeFlow",
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        FunctionName = "FakeFunction",
                        FunctionAssemblyName = "ExampleTest",
                        FunctionFullyQualifiedName = "Runic.ExampleTest.Functions.FakeFunction"
                    }
                }
            });

            Console.WriteLine("here");
            var agentTask = agent.SetThreadLevel(new SetThreadLevelRequest()
            {
                FlowName = "FakeFlow",
                ThreadLevel = 1
            }, cts.Token);

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
