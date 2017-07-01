using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.UnitTest.TestUtility;
using StatsN;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class TestStatsd
    {
        private TestEnvironment _testEnvironment { get; set; }

        [TestInitialize]
        public void Init()
        {
            _testEnvironment = new TestEnvironment();
        }

        [Ignore]
        [TestMethod]
        public void TestBucketCreate()
        {
            IStatsd statsd = Statsd.New<Udp>(options =>
            {
                options.Port = AgentConfiguration.Instance.StatsdPort;
                options.HostOrIp = AgentConfiguration.Instance.StatsdHost;
                options.Prefix = AgentConfiguration.Instance.StatsdPrefix;
            });
            
            statsd.Gauge("debugging.gauges.testcount", 300);
            statsd.Count("debugging.counts.testcount");
            statsd.Gauge("debugging.gauges.testcount", 250);
            statsd.Count("debugging.counts.testcount");
            statsd.Gauge("debugging.gauges.testcount", 0);
        }
    }
}
