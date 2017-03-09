using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RawRabbit.Context;
using Runic.Agent.Messaging;
using Runic.Agent.Service;
using Runic.Core.Attributes;
using Runic.Core.Models;

namespace Runic.Agent.UnitTest
{
    public class TestAgentService
    {
        [BeforeEach]
        public void Init()
        {
            IoC.RegisterDependencies(new Startup());
        }

        [Test]
        public async Task TestSetThreadLevel()
        {
            var agent = new AgentService();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            await agent.AddUpdateFlow(new Flow()
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
            }, cts.Token);
            var agentTask = agent.SetThreadLevel(new SetThreadLevelRequest()
            {
                FlowName = "FakeFunction",
                ThreadLevel = 1
            }, cts.Token);

            Assert.AreEqual(1, agent.GetThreadLevel("FakeFunction"));

            try
            {
                cts.Cancel();
                await agentTask;
            }
            catch (TaskCanceledException)
            {
                
            }
        }
    }
}
