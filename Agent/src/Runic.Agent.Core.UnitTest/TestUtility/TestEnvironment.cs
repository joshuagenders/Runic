using Microsoft.Extensions.Logging;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Framework.Clients;
using System.IO;

namespace Runic.Agent.Core.UnitTest.TestUtility
{
    public class TestEnvironment
    {
        public Mock<IPluginManager> PluginManager { get; set; }
        public Mock<IStatsClient> Stats { get; set; }
        public Mock<IDataService> DataService { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public TestEnvironment()
        {
            Stats = new Mock<IStatsClient>();
            DataService = new Mock<IDataService>();
            LoggerFactory = new LoggerFactory();
            PluginManager = new Mock<IPluginManager>();

        }        
    }
}
