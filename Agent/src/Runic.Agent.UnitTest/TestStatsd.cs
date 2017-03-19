using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Configuration;
using StatsN;
using System;
using System.Collections.Generic;
using System.Text;

namespace Runic.Agent.UnitTest
{
    [TestClass]
    public class TestStatsd
    {
        [TestMethod]
        public void TestBucketCreate()
        {
            var cli = new[]
           {
                "Agent:MaxThreads=321",
                "Agent:LifetimeSeconds=123",
                "Client:MQConnectionString=MyExampleConnection",
                "Statsd:Port=8125",
                "Statsd:Host=192.168.99.100",
                "Statsd:Prefix=Runic.Stats."
            };
            AgentConfiguration.LoadConfiguration(cli);

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
