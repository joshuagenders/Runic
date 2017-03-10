using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RawRabbit.Context;
using Runic.Agent.Configuration;
using Runic.Agent.FlowManagement;
using Runic.Agent.Messaging;
using Runic.Agent.Service;
using Runic.Core.Attributes;
using Runic.Core.Models;

namespace Runic.Agent.UnitTest
{
    public class TestAgentService
    {
        [Test]
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

        [Test]
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

        [Test]
        public void TestExecutionContext()
        {
            var executionContext = new Service.ExecutionContext();
            Assert.Greater(executionContext.MaxThreadCount, 0);
        }
    }
}
