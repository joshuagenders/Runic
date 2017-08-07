using System;
using Runic.Framework.Clients;
using StatsN;

namespace Runic.Agent.Worker.Clients
{
    public class StatsClient : IStatsClient
    {
        private readonly IStatsd _statsd;

        public StatsClient(IStatsd statsd)
        {
            _statsd = statsd;
        }

        public void CountPluginLoaded()
        {
            _statsd?.Count("plugins.pluginLoaded");
        }

        public void CountFlowAdded(string flowName)
        {
            _statsd?.Count($"flows.{flowName}.AddOrUpdated");
        }

        public void CountFunctionSuccess(string functionName)
        {
            _statsd?.Count($"functions.{functionName}.actions.results.execute.success");
        }

        public void CountFunctionFailure(string functionName)
        {
            _statsd?.Count($"functions.{functionName}.actions.results.execute.success");
        }

        public void SetThreadLevel(string flowName, int threadCount)
        {
            _statsd?.Gauge($"flows.{flowName}.threadLevel", threadCount);
        }

        public void CountHttpRequestSuccess(string functionName, string responseCode)
        {
            _statsd?.Count($"functions.{functionName}.http.success.{responseCode}");
        }

        public void CountHttpRequestFailure(string functionName, string responseCode)
        {
            _statsd?.Count($"functions.{functionName}.http.failure.{responseCode}");
        }

        public void Time(string functionName, string actionName, Action actionToTime)
        {
            _statsd?.Timing($"functions.{functionName}.actions.timings.{actionName}", actionToTime);
        }

        public void Time(string functionName, string actionName, int millisecondsEllapsed)
        {
            _statsd?.Timing($"functions.{functionName}.actions.timings.{actionName}", millisecondsEllapsed);
        }
    }
}
