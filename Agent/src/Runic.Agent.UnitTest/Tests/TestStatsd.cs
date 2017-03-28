using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Configuration;
using Runic.Agent.UnitTest.TestUtility;
using StatsN;

namespace Runic.Agent.UnitTest.Tests
{
    [TestClass]
    public class TestStatsd
    {
        private AgentWorld _world { get; set; }

        [TestInitialize]
        public void Init()
        {
            _world = new AgentWorld();
        }

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
