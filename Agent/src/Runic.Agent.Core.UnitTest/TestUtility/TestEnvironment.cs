using Microsoft.Extensions.Logging;
using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.Services.Interfaces;
using Runic.Framework.Clients;

namespace Runic.Agent.Core.UnitTest.TestUtility
{
    public class TestEnvironment
    {
        public Mock<IPluginManager> PluginManager { get; set; }
        public Mock<IStatsClient> Stats { get; set; }
        public Mock<IDataService> DataService { get; set; }
        public Mock<IRunnerService> RunnerService { get; set; }
        public Mock<IDatetimeService> DatetimeService { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public TestEnvironment()
        {
            Stats = new Mock<IStatsClient>();
            DataService = new Mock<IDataService>();
            LoggerFactory = new LoggerFactory();
            PluginManager = new Mock<IPluginManager>();
            DatetimeService = new Mock<IDatetimeService>();
            RunnerService = new Mock<IRunnerService>();
        }        
    }
}
