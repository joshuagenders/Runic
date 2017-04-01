using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Harness;
using Runic.Agent.UnitTest.TestUtility;
using Runic.Framework.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestThreadManager
    {
        private AgentWorld _world { get; set; }

        [TestInitialize]
        public void Init()
        {
            _world = new AgentWorld();
        }

        [TestMethod]
        public async Task TestUpdateThreads()
        {
            var flow = new Flow()
            {
                Name = "Test",
                StepDelayMilliseconds = 100,
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step1",
                        Function = new FunctionInformation()
                        {
                            AssemblyName = "Runic.ExampleTest",
                            FunctionName = "AsyncWait",
                            AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.FakeFunction"
                        },
                        Repeat = 1,
                        EvaluateSuccessOnRepeat = false //todo
                    }
                }
            };

            var manager = new ThreadManager(flow, _world.PluginManager, _world.Stats, _world.DataService);
            await manager.SafeUpdateThreadCountAsync(1);
            Assert.AreEqual(1, manager.GetCurrentThreadCount());
            await manager.SafeUpdateThreadCountAsync(0);
            Assert.AreEqual(0, manager.GetCurrentThreadCount());
        }
    }
}
