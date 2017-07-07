using Microsoft.Extensions.Logging;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Core.Metrics;
using Runic.Framework.Clients;
using System;
using System.IO;

namespace Runic.Agent.Core.UnitTest.TestUtility
{
    public class TestEnvironment
    {
        public IPluginManager PluginManager { get; set; }
        public Mock<IStatsClient> Stats { get; set; }
        public Mock<IDataService> DataService { get; set; }
        public TestEnvironment()
        {
            Stats = new Mock<IStatsClient>();
            DataService = new Mock<IDataService>();
            PluginManager = new PluginManager(
                new Mock<IRuneClient>().Object, 
                new FilePluginProvider(Directory.GetCurrentDirectory()), 
                Stats.Object,
                new LoggerFactory());

        }        
    }
}
